using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Inlämning1
{
    public class AppDbContext : DbContext
    {
        public DbSet<Movie> Movie { get; set; }
        public DbSet<Screening> Screening { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=(local)\SQLEXPRESS;Initial Catalog=DataAccessConsoleAssignment;Integrated Security=True");
        }
    }
    public class Movie
    {
        public int ID { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        [Column(TypeName = "date")]
        public DateTime ReleaseDate { get; set; }

        public List<Screening> Screenings { get; set; }
    }

    public class Screening
    {
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        [Required]
        public Movie Movie { get; set; }
        public Int16 Seats { get; set; }
    }
    public class Program
    {
        private static AppDbContext database;
        public static void Main()
        {
            using (database = new AppDbContext())
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                bool running = true;
                while (running)
                {
                    int selected = Utils.ShowMenu("What do you want to do?", new[] {
                    "List Movies",
                    "Add Movie",
                    "Delete Movie",
                    "Load Movies from CSV File",
                    "List Screenings",
                    "Add Screening",
                    "Delete Screening",
                    "Exit"
                });
                    Console.Clear();

                    if (selected == 0) ListMovies();
                    else if (selected == 1) AddMovie();
                    else if (selected == 2) DeleteMovie();
                    else if (selected == 3) LoadMoviesFromCSVFile();
                    else if (selected == 4) ListScreenings();
                    else if (selected == 5) AddScreening();
                    else if (selected == 6) DeleteScreening();
                    else running = false;

                    Console.WriteLine();
                }
            }
        }

        public static void ListMovies()
        {
            Utils.WriteHeading("Movies:");

            if (database.Movie.Count() == 0)
            {
                Console.WriteLine("There are no movies.");
            }
            else
            {
                foreach (var movie in database.Movie.OrderBy(m => m.Title))
                {
                    Console.WriteLine("- " + movie.Title + " (" + movie.ReleaseDate.Year + ")");
                }
            }
        }

        public static void AddMovie()
        {
            Utils.WriteHeading("Add Movie");

            Movie movie = new Movie();
            movie.Title = Utils.ReadString("Title:");
            movie.ReleaseDate = Utils.ReadDate("Release Date:");

            database.Add(movie);
            database.SaveChanges();

            Console.Clear();

            Console.WriteLine($"{movie.Title} ({movie.ReleaseDate.Year}) has been added.");
        }

        public static void DeleteMovie()
        {
            if (database.Movie.Count() == 0)
            {
                Utils.WriteHeading("Delete Movie:");
                Console.WriteLine("There are no movies.");
            }
            else
            {
                string[] movieTitles = database.Movie.OrderBy(m => m.Title).Select(m => m.Title).ToArray();
                int movieIndex = Utils.ShowMenu("Delete Movie:", movieTitles);
                string movieTitle = movieTitles[movieIndex];
                var movie = database.Movie.OrderBy(m => m.Title).Skip(movieIndex).First();

                database.Remove(movie);
                database.SaveChanges();

                Console.Clear();

                Console.WriteLine(movie.Title + " (" + movie.ReleaseDate.Year + ") has been deleted.");
            }
        }

        public static void LoadMoviesFromCSVFile()
        {
            int selected = Utils.ShowMenu("All existing movies will be deleted. Are you sure?", new[] {
                "Yes",
                "No"
            });
            if (selected == 0)
            {
                database.Movie.RemoveRange(database.Movie);
                database.Screening.RemoveRange(database.Screening);
                //database.SaveChanges();

                Console.Write("Enter path to CSV file: ");
                string filepath = Console.ReadLine();

                var movies = new Dictionary<int, Movie>();
                string[] lines = File.ReadAllLines(filepath).ToArray();
                foreach (string line in lines)
                {
                    try
                    {
                        string[] values = line.Split(',').Select(v => v.Trim()).ToArray();

                        string title = values[0];
                        DateTime releaseDate = Convert.ToDateTime(values[1]);

                        Movie movie = new Movie
                        {
                            Title = title,
                            ReleaseDate = releaseDate
                        };

                        database.Add(movie);
                    }
                    catch
                    {
                        Console.WriteLine("Could not read movie: " + line);
                    }
                }
                database.SaveChanges();
            }
            else
            {
                return;
            }
        }

        public static void ListScreenings()
        {
            Utils.WriteHeading("Screenings:");

            if (database.Screening.Count() == 0)
            {
                Console.WriteLine("There are no screenings.");
            }
            else
            {
                foreach (var screening in database.Screening.Include(s => s.Movie).OrderBy(s => s.DateTime))
                {
                    Console.WriteLine($"- {screening.DateTime}: {screening.Movie.Title} ({screening.Seats} seats)");
                }
            }
        }

        public static void AddScreening()
        {
            Utils.WriteHeading("Add Screening");

            string[] movieTitles = database.Movie.Select(m => m.Title).ToArray();
            int movieIndex = Utils.ShowMenu("Movie:", movieTitles);
            string movieTitle = movieTitles[movieIndex];
            var movie = database.Movie.Skip(movieIndex).First();

            Console.WriteLine();

            DateTime date = Utils.ReadFutureDate("Day");

            string time = Utils.ReadString("Time (HH:MM):");
            int hour = int.Parse(time.Substring(0, 2));
            int minute = int.Parse(time.Substring(3, 2));

            DateTime datetime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);

            int seats = Utils.ReadInt("Seats:");

            Screening screening = new Screening
            {
                DateTime = datetime,
                Movie = movie,
                Seats = (short)seats
            };

            database.Add(screening);
            database.SaveChanges();

            Console.Clear();

            Console.WriteLine($"{movie.Title} on {screening.DateTime} ({screening.Seats} seats) has been added.");
        }

        public static void DeleteScreening()
        {
            if (database.Screening.Count() == 0)
            {
                Utils.WriteHeading("Delete Screening:");
                Console.WriteLine("There are no screenings.");
            }
            else
            {
                List<string> screenings = new List<string>();
                foreach (var screening in database.Screening.Include(s => s.Movie).OrderBy(s => s.DateTime))
                {
                    screenings.Add(screening.Movie.Title + " on " + screening.DateTime + " (" + screening.Seats + " seats)");
                }

                int screeningIndex = Utils.ShowMenu("Delete Screening:", screenings.ToArray());
                string movieTitle = screenings[screeningIndex];
                var cancelledScreening = database.Screening.OrderBy(s => s.DateTime).Skip(screeningIndex).First();

                database.Remove(cancelledScreening);
                database.SaveChanges();

                Console.Clear();

                Console.WriteLine($"{cancelledScreening.Movie.Title} on {cancelledScreening.DateTime} ({cancelledScreening.Seats} seats) has been deleted.");
            }
        }
    }

    public static class Utils
    {
        public static string ReadString(string prompt)
        {
            Console.Write(prompt + " ");
            string input = Console.ReadLine();
            return input;
        }

        public static int ReadInt(string prompt)
        {
            Console.Write(prompt + " ");
            int input = int.Parse(Console.ReadLine());
            return input;
        }

        public static DateTime ReadDate(string prompt)
        {
            Console.WriteLine(prompt);
            int year = ReadInt("Year:");
            int month = ReadInt("Month:");
            int day = ReadInt("Day:");
            var date = new DateTime(year, month, day);
            return date;
        }

        public static DateTime ReadFutureDate(string prompt)
        {
            var dates = new[]
            {
                DateTime.Now.Date,
                DateTime.Now.AddDays(1).Date,
                DateTime.Now.AddDays(2).Date,
                DateTime.Now.AddDays(3).Date,
                DateTime.Now.AddDays(4).Date,
                DateTime.Now.AddDays(5).Date,
                DateTime.Now.AddDays(6).Date,
                DateTime.Now.AddDays(7).Date
            };
            var wordOptions = new[] { "Today", "Tomorrow" };
            var nameOptions = dates.Skip(2).Select(d => d.DayOfWeek.ToString());
            var options = wordOptions.Concat(nameOptions);
            int daysAhead = ShowMenu(prompt, options.ToArray());
            var selectedDate = dates[daysAhead];
            return selectedDate;
        }

        public static void WriteHeading(string text)
        {
            Console.WriteLine(text);
            string underline = new string('-', text.Length);
            Console.WriteLine(underline);
        }

        public static int ShowMenu(string prompt, string[] options)
        {
            if (options == null || options.Length == 0)
            {
                throw new ArgumentException("Cannot show a menu for an empty array of options.");
            }

            Console.WriteLine(prompt);

            int selected = 0;

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                // If this is not the first iteration, move the cursor to the first line of the menu.
                if (key != null)
                {
                    Console.CursorLeft = 0;
                    Console.CursorTop = Console.CursorTop - options.Length;
                }

                // Print all the options, highlighting the selected one.
                for (int i = 0; i < options.Length; i++)
                {
                    var option = options[i];
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine("- " + option);
                    Console.ResetColor();
                }

                // Read another key and adjust the selected value before looping to repeat all of this.
                key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Length - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }
            }

            // Reset the cursor and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
    }
}

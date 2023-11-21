# Movie Screening Management Console Application

* "Inlämning" means submission.

This console application is designed to manage movie screenings by allowing users to perform various actions such as adding, deleting, and listing movies and screenings. It utilizes Microsoft.EntityFrameworkCore to interact with a SQL Server database.
Features

    List Movies: View a list of movies along with their release years.
    Add Movie: Add a new movie to the database, providing its title and release date.
    Delete Movie: Remove a movie from the database.
    Load Movies from CSV File: Bulk addition of movies from a CSV file.
    List Screenings: Display a list of screenings, including movie titles and available seats.
    Add Screening: Schedule a new screening for a movie.
    Delete Screening: Remove a scheduled screening.

## Setup
### Prerequisites

    Database: SQL Server (local)\SQLEXPRESS with Integrated Security.

### Configuration

    The AppDbContext class specifies the database connection string.
    Modify the connection string in OnConfiguring method if needed: options.UseSqlServer("Your_Connection_String_Here");

### Running the Application

    Run the application, and a menu will prompt you with various actions to choose from.

## Usage

    Navigate the menu by using the arrow keys to select options.
    Input required details when prompted to perform actions such as adding or deleting movies/screenings.

## Note

    The application is designed for local use with a SQL Server database.
    Ensure proper input formats when adding movies or scheduling screenings (e.g., date, time).
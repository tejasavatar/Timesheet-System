using System;
using MySql.Data.MySqlClient;

namespace Timesheet
{
    internal class Program
    {
        // Define the connection string to the MySQL database
        private const string connectionString = ""; // Add your server, port, database name, uid and password. For privacy reasons, I have removed my connection string

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to NASA'S Timesheet System");

            // Establish a connection to the MySQL database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // Open the database connection
                connection.Open();
                // Print a message indicating successful connection
                Console.WriteLine("Connected to MySQL database!");

                bool exit = false;
                while (!exit)
                {
                    Console.WriteLine("\nMenu:");
                    Console.WriteLine("1. Insert Timesheet Information");
                    Console.WriteLine("2. Select Timesheet Information");
                    Console.WriteLine("3. Update Timesheet Information");
                    Console.WriteLine("4. Delete Timesheet Information");
                    Console.WriteLine("5. Exit");

                    Console.Write("Enter your choice: ");
                    int choice;
                    if (!int.TryParse(Console.ReadLine(), out choice))
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                        continue;
                    }

                    switch (choice)
                    {
                        case 1:
                            InsertTimesheetInformation(connection);
                            break;
                        case 2:
                            SelectTimesheetInformation(connection);
                            break;
                        case 3:
                            UpdateTimesheetInformation(connection);
                            break;
                        case 4:
                            DeleteTimesheetInformation(connection);
                            break;
                        case 5:
                            exit = true;
                            Console.WriteLine("Exiting program.");
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.");
                            break;
                    }
                }
            }
        }

        private static void InsertTimesheetInformation(MySqlConnection connection)
        {
            try
            {
                // Prompt user for information
                Console.Write("Enter your 7-digit ID: ");
                int userId = int.Parse(Console.ReadLine());

                // Check if the user ID already exists
                if (IsUserIdExists(connection, userId))
                {
                    Console.WriteLine("User ID already exists. Please enter a unique ID.");
                    return; // Exit the method to prevent further execution
                }

                Console.Write("Enter your first name: ");
                string firstName = Console.ReadLine();

                Console.Write("Enter your last name: ");
                string lastName = Console.ReadLine();

                // Define the SQL query to insert user information into the Users table
                string insertUserQuery = "INSERT INTO Users (UserID, FirstName, LastName) VALUES (@userId, @firstName, @lastName)";
                // Create a MySqlCommand object with the insert query and connection
                MySqlCommand insertUserCommand = new MySqlCommand(insertUserQuery, connection);
                // Add parameters to the command for user ID, first name, and last name
                insertUserCommand.Parameters.AddWithValue("@userId", userId);
                insertUserCommand.Parameters.AddWithValue("@firstName", firstName);
                insertUserCommand.Parameters.AddWithValue("@lastName", lastName);
                // Execute the insert query to add user information to the Users table
                insertUserCommand.ExecuteNonQuery();

                // Prompt user for timesheet information
                Console.Write("Enter the day of the week (ex. Monday, Tuesday, etc.): ");
                string dayOfWeek = Console.ReadLine();

                Console.Write("Enter start time (ex. 9:00, Enter Numbers only, 12 hour scale): ");
                string startTimeStr = Console.ReadLine();
                TimeSpan startTime = TimeSpan.Parse(startTimeStr);

                Console.Write("Enter end time (ex. 5:00, Enter Numbers only, 12 hour scale): ");
                string endTimeStr = Console.ReadLine();
                TimeSpan endTime = TimeSpan.Parse(endTimeStr);

                // Calculate the total hours worked
                double totalHours = (endTime - startTime).TotalHours;

                // Define the SQL query to insert timesheet information into the database
                string insertTimesheetQuery = "INSERT INTO Timesheet (UserID, DayOfWeek, StartTime, EndTime) VALUES (@userId, @dayOfWeek, @startTime, @endTime)";
                // Create a MySqlCommand object with the insert query and connection
                MySqlCommand insertTimesheetCommand = new MySqlCommand(insertTimesheetQuery, connection);
                // Add parameters to the command for user ID, day of the week, start time, and end time
                insertTimesheetCommand.Parameters.AddWithValue("@userId", userId);
                insertTimesheetCommand.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);
                insertTimesheetCommand.Parameters.AddWithValue("@startTime", startTime);
                insertTimesheetCommand.Parameters.AddWithValue("@endTime", endTime);
                // Execute the insert query to add timesheet information to the database
                insertTimesheetCommand.ExecuteNonQuery();

                // Define the SQL query to update total hours worked in the database
                string updateTotalHoursQuery = "INSERT INTO TotalHours (UserID, TotalHours) VALUES (@userId, @totalHours) ON DUPLICATE KEY UPDATE TotalHours = TotalHours + @totalHours";
                // Create a MySqlCommand object with the update query and connection
                MySqlCommand updateTotalHoursCommand = new MySqlCommand(updateTotalHoursQuery, connection);
                // Add parameters to the command for user ID and total hours
                updateTotalHoursCommand.Parameters.AddWithValue("@userId", userId);
                updateTotalHoursCommand.Parameters.AddWithValue("@totalHours", totalHours);
                // Execute the update query to update total hours worked in the database
                updateTotalHoursCommand.ExecuteNonQuery();

                // Print a success message indicating that timesheet information was stored successfully in the NYITDatabase
                Console.WriteLine("Timesheet information stored successfully!");
            }
            catch (Exception ex)
            {
                // Print an error message with details of the exception
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static bool IsUserIdExists(MySqlConnection connection, int userId)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE UserID = @userId";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }

        private static void SelectTimesheetInformation(MySqlConnection connection)
        {
            try
            {
                // Define the SQL query to select timesheet information
                string selectQuery = "SELECT * FROM Timesheet";

                // Create a MySqlCommand object with the select query and connection
                MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);

                // Execute the select query and get a data reader
                using (MySqlDataReader reader = selectCommand.ExecuteReader())
                {
                    // Check if there are any rows returned
                    if (reader.HasRows)
                    {
                        Console.WriteLine("\nTimesheet Information:");
                        // Loop through the rows and print each timesheet record
                        while (reader.Read())
                        {
                            Console.WriteLine($"UserID: {reader["UserID"]}, DayOfWeek: {reader["DayOfWeek"]}, StartTime: {reader["StartTime"]}, EndTime: {reader["EndTime"]}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No timesheet information found.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Print an error message with details of the exception
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void UpdateTimesheetInformation(MySqlConnection connection)
        {
            try
            {
                // Define the SQL query to update timesheet information
                string updateQuery = "UPDATE Timesheet SET DayOfWeek = @dayOfWeek WHERE UserID = @userId";

                // Create a MySqlCommand object with the update query and connection
                MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);

                // Prompt user for information
                Console.Write("Enter the user ID whose timesheet you want to update: ");
                int userId = int.Parse(Console.ReadLine());

                Console.Write("Enter the new day of the week: ");
                string newDayOfWeek = Console.ReadLine();

                // Add parameters to the command for user ID and new day of the week
                updateCommand.Parameters.AddWithValue("@userId", userId);
                updateCommand.Parameters.AddWithValue("@dayOfWeek", newDayOfWeek);

                // Execute the update query
                int rowsAffected = updateCommand.ExecuteNonQuery();

                // Check if any rows were affected
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Timesheet information updated successfully.");
                }
                else
                {
                    Console.WriteLine("No timesheet information updated. User ID may not exist.");
                }
            }
            catch (Exception ex)
            {
                // Print an error message with details of the exception
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void DeleteTimesheetInformation(MySqlConnection connection)
        {
            try
            {
                // Prompt user for information
                Console.Write("Enter the user ID whose timesheet you want to delete: ");
                int userId = int.Parse(Console.ReadLine());

                // Define the SQL query to fetch user information
                string selectQuery = "SELECT Users.FirstName, Users.LastName " +
                                     "FROM Users " +
                                     "WHERE Users.UserID = @userId";

                // Create a MySqlCommand object with the select query and connection
                MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);

                // Add parameter to the command for user ID
                selectCommand.Parameters.AddWithValue("@userId", userId);

                // Variables to store user information
                string firstName = "";
                string lastName = "";

                // Execute the select query and get a data reader
                using (MySqlDataReader reader = selectCommand.ExecuteReader())
                {
                    // Check if there are any rows returned
                    if (reader.HasRows)
                    {
                        // Read the first row (should only be one row)
                        reader.Read();

                        // Extract user information
                        firstName = reader["FirstName"].ToString();
                        lastName = reader["LastName"].ToString();
                    }
                    else
                    {
                        Console.WriteLine("No user information found for the provided user ID.");
                        return; // Exit the method
                    }
                } // DataReader is automatically closed when exiting the using block

                // Print the information to confirm with the user
                Console.WriteLine($"You are about to delete all timesheet information for:");
                Console.WriteLine($"User ID: {userId}, First Name: {firstName}, Last Name: {lastName}");

                // Prompt for confirmation
                Console.Write("Are you sure you want to delete this user and all associated timesheet information? (Y/N): ");
                string confirmation = Console.ReadLine();

                if (confirmation.ToUpper() == "Y")
                {
                    // Delete timesheet entries
                    DeleteTimesheetEntries(connection, userId);

                    // Delete total hours entry
                    DeleteTotalHoursEntry(connection, userId);

                    // Delete user entry
                    DeleteUserEntry(connection, userId);

                    Console.WriteLine("User and associated timesheet information deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Deletion cancelled.");
                }
            }
            catch (Exception ex)
            {
                // Print an error message with details of the exception
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void DeleteTimesheetEntries(MySqlConnection connection, int userId)
        {
            try
            {
                // Define the SQL query to delete timesheet information
                string deleteQuery = "DELETE FROM Timesheet WHERE UserID = @userId";

                // Create a MySqlCommand object with the delete query and connection
                MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection);

                // Add parameter to the command for user ID
                deleteCommand.Parameters.AddWithValue("@userId", userId);

                // Execute the delete query
                deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Print an error message with details of the exception
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void DeleteTotalHoursEntry(MySqlConnection connection, int userId)
        {
            try
            {
                // Define the SQL query to delete total hours entry
                string deleteQuery = "DELETE FROM TotalHours WHERE UserID = @userId";

                // Create a MySqlCommand object with the delete query and connection
                MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection);

                // Add parameter to the command for user ID
                deleteCommand.Parameters.AddWithValue("@userId", userId);

                // Execute the delete query
                deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Print an error message with details of the exception
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void DeleteUserEntry(MySqlConnection connection, int userId)
        {
            try
            {
                // Define the SQL query to delete user entry
                string deleteQuery = "DELETE FROM Users WHERE UserID = @userId";

                // Create a MySqlCommand object with the delete query and connection
                MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection);

                // Add parameter to the command for user ID
                deleteCommand.Parameters.AddWithValue("@userId", userId);

                // Execute the delete query
                deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Print an error message with details of the exception
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}

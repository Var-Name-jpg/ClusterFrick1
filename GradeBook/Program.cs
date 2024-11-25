/*
 * This program is possibly one of the biggest and most complex databases I've made
 * Teachers will be able to import and exports student's names, IDs, and grades to and from locally saved .csv files
 * The idea here is that you can create AND delete students (as a whole), grades and individual gradebook files
 * I have seperate .csv files for each student so it is easier to print off a student's "Report Card" without having to sift through one giant file
 * Goodluck deciphering this
 * :)
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Channels;

namespace GradeBook
{
    public class Program
    {
        // Gradebook creation
        static Gradebook studentGradeBook = new Gradebook(new Dictionary<string, Student>());
        static string gradeBookPath = Path.Combine(Directory.GetCurrentDirectory(), "GradeBook.csv"); // Giant gradebook with averages

        // Method that populates each student with their appropriate grades from the gradebook .csv file
        public static void PopulateStudentGrades(string path)
        {
            // If there is anything in the student dictionary...
            if (studentGradeBook.Students.Any())
            {
                // Loop for every student
                foreach (var student in studentGradeBook.Students)
                {

                    // Basic streamreader class
                    using (StreamReader sr = new StreamReader(path))
                    {

                        sr.ReadLine(); // Skip Header

                        // empty string for each line
                        string line = string.Empty;

                        // While the line that StreamReader is reading IS NOT null...
                        while ((line = sr.ReadLine()) != null)
                        {
                            // turn the line (str) into an array to be easier acceessed
                            string[] gradeInfoArray = line.Split(",");

                            // Just in case
                            try
                            {
                                // Add the subject (str) and the grade for that subject (float) to the student's grade dictionary
                                student.Value.AddGrade(gradeInfoArray[0], float.Parse(gradeInfoArray[1]));
                            }
                            catch
                            {
                                Console.WriteLine("There was an internal error, please try again.");
                            }
                        }
                    }
                }
            }
        }

        // Method that creates a grade file for a student
        public static void CreateGradesFile(string studentId, string path)
        {

            // Basic streamwriter statement
            using (StreamWriter sw = File.CreateText(path))
            {
                // After file creation, write the header in the file
                sw.WriteLine("Subject,Grade");
            }
        }

        // Method that populates the gradebook dictionary from the gradebook .csv file
        public static void PopulateStudents()
        {
            // using StreamReader Statement
            using (StreamReader sr = new StreamReader(gradeBookPath))
            {
                sr.ReadLine(); // Skip the header

                // Empty string for the lines being read
                string line = string.Empty;

                // While the line being read IS NOT null
                while ((line = sr.ReadLine()) != null)
                {
                    // Turn the line being read into an array to be more accessable
                    string[] studentInfoArray = line.Split(',');
                    
                    // Create a new student object for the studentsByName dictionary
                    studentGradeBook.Students.Add(
                        studentInfoArray[0], // Name (str)
                        new Student(studentInfoArray[1], studentInfoArray[0]) // Student Object
                    );
                }
            }
        }

        // Method that creates a gradebook file (in case one doesnt exist)
        public static void CreateGradeBookFile()
        {
            // using StreamWriter statement
            using (StreamWriter sw = File.CreateText(gradeBookPath))
            {
                // After file creation, write the header to the file
                sw.WriteLine("Student Name,Student ID,Grade Average");
            }
        }

        // Method that exports a student to the GradeBook.csv file
        public static void ExportStudentsToGradeBook(Dictionary<string, Student> students)
        {
            // Gradebook life cycle
            File.Delete(gradeBookPath);
            CreateGradeBookFile();

            using (StreamWriter sw = File.AppendText(gradeBookPath))
            {
                foreach (var student in students)
                {
                    sw.WriteLine($"{student.Value.Name},{student.Value.Student_Id},{student.Value.CalculateAverageGrade()}");
                }
            }
        }

        // Method that exports all students grades to a .csv file
        public static void ExportAllStudentsGradesToFile(Dictionary<string, Student> students)
        {
            foreach (var student in students)
            {
                string tempPath = Path.Combine(Directory.GetCurrentDirectory(), $"StudentGrades\\{student.Value.Name}_{student.Key}");

                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                    CreateGradesFile(student.Key, tempPath);

                    using (StreamWriter sw = File.CreateText(tempPath))
                    {
                        foreach (var grade in student.Value.Grades)
                        {
                            sw.WriteLine($"{grade.Key},{grade.Value}");
                        }
                    }

                }
            }
        }

        // Main method
        public static void Main(string[] args)
        {
            // Import Gradebook or Create Gradebook
            if (File.Exists(gradeBookPath)) { PopulateStudents(); }
            else { CreateGradeBookFile(); }

            // Import Student Grades or Create Grades File
            foreach (var student in studentGradeBook.Students)
            {
                string tempPath = Path.Combine(Directory.GetCurrentDirectory(), $"StudentGrades\\{student.Value.Name}_{student.Key}");

                if (File.Exists(tempPath)) { PopulateStudentGrades(tempPath); }
                else { CreateGradesFile(student.Key, tempPath); }
            }

            while (true)
            {
                // Show user the options
                Console.WriteLine("\nPlease Choose An Option (enter numerical values only)");
                Console.WriteLine("1. Add a New Student");
                Console.WriteLine("2. Delete an Existing Student");
                Console.WriteLine("3. Add Grades for Existing Student");
                Console.WriteLine("4. View an Existing Student's Record");
                Console.WriteLine("5. Show the Student With the Highest Grade Average");
                Console.WriteLine("6. Save everything to the gradebook");
                Console.WriteLine("7. Exit Program");
                Console.Write(">> ");

                // try and Catch
                // If theres any errors the program can continue running
                // Also for input validity
                try
                {
                    string userInput = Console.ReadLine();

                    switch (userInput)
                    {
                        case "1":
                            bool _valid = false;

                            while (!_valid)
                            {
                                // Student Name Input
                                Console.Write("\nPlease enter the student's first name: ");
                                string firstName = Console.ReadLine().Trim();
                                Console.Write("Please enter the studen's last name: ");
                                string lastName = Console.ReadLine().Trim();
                                string fullName = $"{firstName} {lastName}";

                                // Student ID Generator
                                int idCount = 0;

                                // If the list is empty, instead of throwing an error just test it
                                // (don't say it doesn't happen because it fucking did and I lost my mind)
                                if (studentGradeBook.Students.Any())
                                {
                                    // Loop through all existing students and increment the idcount
                                    foreach (var student in studentGradeBook.Students)
                                    {
                                        idCount++;
                                    }
                                }

                                // Convert the ID to a string
                                string stringId = (idCount+1).ToString();


                                // Promt user asking if the information is correct
                                Console.WriteLine("Is this information correct: (y/n)");
                                Console.WriteLine($"Name: {fullName}\nID: {stringId}");
                                Console.Write(">> ");

                                // if yes then add them
                                switch (Console.ReadLine().ToLower())
                                {
                                    case "y":
                                        // Add a student with the ID and Name
                                        studentGradeBook.AddStudent(new Student(stringId, fullName));
                                        Console.WriteLine($"\nStudent added successfully with ID: {stringId}...");
                                        CreateGradesFile(stringId, Path.Combine(Directory.GetCurrentDirectory(), $"StudentGrades\\{fullName}_{stringId}.csv"));
                                        Console.WriteLine($"Student grades file created successfully.\n");
                                        _valid = true;
                                        break;

                                    case "n":
                                        break;

                                    default:
                                        Console.WriteLine("Inalid input.\n");
                                        break;
                                }

                                if (_valid) { break; }

                            }
                            break;

                        case "2":
                            _valid = false;

                            while (!_valid)
                            {
                                Console.Write("Please enter the student's ID: ");
                                userInput = Console.ReadLine();

                                foreach (string student in studentGradeBook.Students.Keys)
                                {
                                    if (student == userInput)
                                    {
                                        studentGradeBook.RemoveStudent(student);
                                        _valid = true;
                                        break;
                                    }
                                }

                                if (_valid) { break; }
                                else { Console.WriteLine("That ID does not exist in the system."); }
                            }

                            break;

                        case "6":
                            if (studentGradeBook.Students.Any())
                            {
                                ExportStudentsToGradeBook(studentGradeBook.Students);
                                ExportAllStudentsGradesToFile(studentGradeBook.Students);
                            }
                            break;

                        case "7":
                            if (studentGradeBook.Students.Any())
                            {
                                ExportStudentsToGradeBook(studentGradeBook.Students);
                                ExportAllStudentsGradesToFile(studentGradeBook.Students);
                            }
                            System.Environment.Exit(0);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}\n");
                }
            }
            
        }
    }
}
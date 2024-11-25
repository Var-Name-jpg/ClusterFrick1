using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GradeBook
{
    public class Student
    {
        // Basic variables for the student
        public string Student_Id {  get; set; }
        public string Name { get; set; }
        public Dictionary<string, float> Grades = new Dictionary<string, float>();

        // Main student class w/ atributes
        public Student(string student_Id, string name)
        {
            Student_Id = student_Id;
            Name = name;
        }

        // Method to add a grade to the list
        public void AddGrade(string subject, float grade)
        {
            Grades.Add(subject, grade);
        }

        // Method to calculate the student's overall average grade
        public float CalculateAverageGrade()
        {
            // Number being modified
            float totalGrades = 0f;

            // Add up all the grades
            foreach (float x in Grades.Values)
            {
                totalGrades += x;
            }

            // Divide by the total number of grades
            return ( totalGrades / Grades.Count );
        }

        // Method to return a string representation of the student
        // Formatted for the console
        public string FormatStudentForConsole()
        {
            string studentString = string.Empty;

            studentString += $"Name: {Name}\n";
            studentString += $"ID: {Student_Id}\n";

            for (int i = 0; i < Grades.Count; i++)
            {
                studentString += $"Subject: {Grades.ElementAt(i).Key} | Grade: {Grades.ElementAt(i).Value}\n";
            }

            studentString += $"Total Average Grade: {CalculateAverageGrade()}";

            return studentString;
        }

        // Method to return a string representation of the student
        // Formatted for the .csv file
        public string FormatStudentForCsv()
        {
            return $"{Name},{Student_Id},{CalculateAverageGrade()}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GradeBook
{
    public class Gradebook
    {
        // Full list of students
        public Dictionary<string, Student> Students;

        // Main class method
        public Gradebook(Dictionary<string, Student> students)
        {
            Students = students;
        }

        // Method that adds a student to the Gradebook list
        public void AddStudent(Student student)
        {
            // Add a specific student to the list
            Students.Add(student.Student_Id, student);
        }

        // Method that locates a student based on the student id
        public Student FindStudent(string studentId)
        {
            if (Students.ContainsKey(studentId)) { return Students[studentId]; }

            throw new Exception("Student does not exist in the gradebook.");
        }

        // Method that removes a student based on their student id
        public void RemoveStudent(string studentId)
        {
            if (Students.ContainsKey(studentId)) {  Students.Remove(studentId); }

            throw new Exception("Student does not exist in the gradebook.");
        }

        // Method that displays and prints all the students and their averages
        public void DisplayAllStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine($"Name: {student.Value.Name}\nID: {student.Key}\nGrade Average: {student.Value.CalculateAverageGrade()}");
            }
        }

        // Method that returns the student with the highest overall grade average
        public Student FindHighestGradeAverage()
        {

            if (!Students.Any())
            {
                throw new Exception("There are no students in the gradebook.");
            }

            // Staring variable
            float highestGrade = 0f;
            string highestGradeId = string.Empty;

            foreach (var student in Students.Values)
            {
                if (student.CalculateAverageGrade() > highestGrade)
                {
                    highestGrade = student.CalculateAverageGrade();
                    highestGradeId = student.Student_Id;
                }
            }

            foreach (var student in Students.Values)
            {
                if (student.Student_Id ==  highestGradeId)
                {
                    return student;
                }
            }

            throw new Exception("An internal error has occured, please try again.");
        }
    }
}

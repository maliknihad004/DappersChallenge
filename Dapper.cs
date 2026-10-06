using Dapper;
using Npgsql;
using DotNetEnv;
//CHATGPT Helped me with the connection setup.

public class DapperRepository
{
    private readonly string connectionString;

    public DapperRepository()
    {
        Env.Load();

        connectionString =
            Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
            ?? throw new Exception("Database connection string was not found.");
    }

    public async Task AddStudent(Students student)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.ExecuteAsync(
                "INSERT INTO Students (id, name, email, IsActive) VALUES (@Id, @Name, @Email, TRUE)",
                student
            );
        }
    }

    public async Task AddCourse(Courses course)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.ExecuteAsync(
                "INSERT INTO Courses (id, name, IsActive) VALUES (@Id, @Name, TRUE)",
                course
            );
        }
    }

    public async Task AddEnrollment(Enrollments enrollment)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            var students = await connection.QueryAsync<Students>(
                "SELECT * FROM Students WHERE id = @StudentId AND IsActive = TRUE",
                enrollment
            );

            if (students.Count() == 0)
            {
                Console.WriteLine("Student does not exist or is inactive.");
                return;
            }


            var courses = await connection.QueryAsync<Courses>(
                "SELECT * FROM Courses WHERE id = @CourseId AND IsActive = TRUE",
                enrollment
            );

            if (courses.Count() == 0)
            {
                Console.WriteLine("Course does not exist or is inactive.");
                return;
            }


            var existingEnrollment = await connection.QueryAsync<Enrollments>(
                "SELECT * FROM Enrollments WHERE student_id = @StudentId AND course_id = @CourseId AND semester = @Semester",
                enrollment
            );

            if (existingEnrollment.Count() > 0)
            {
                Console.WriteLine("Student is already enrolled in this course.");
                return;
            }


            var studentEnrollments = await connection.QueryAsync<Enrollments>(
                "SELECT * FROM Enrollments WHERE student_id = @StudentId AND semester = @Semester",
                enrollment
            );

            if (studentEnrollments.Count() >= 3)
            {
                Console.WriteLine(
                    "Student has already enrolled in 3 courses for this semester."
                );
                return;
            }

            var courseEnrollments = await connection.QueryAsync<Enrollments>(
                "SELECT * FROM Enrollments WHERE course_id = @CourseId AND semester = @Semester",
                enrollment
            );

            if (courseEnrollments.Count() >= 8)
            {
                Console.WriteLine(
                    "Course has already reached its enrollment limit."
                );
                return;
            }

            await connection.ExecuteAsync(
                "INSERT INTO Enrollments (id, student_id, course_id, semester) VALUES (@Id, @StudentId, @CourseId, @Semester)",
                enrollment
            );

            Console.WriteLine("Enrollment added.");
        }
    }

    public async Task DeleteStudent(int studentId, string currentSemester)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.ExecuteAsync(
                "UPDATE Students SET IsActive = FALSE WHERE id = @studentId",
                new { studentId }
            );

            await connection.ExecuteAsync(
                "DELETE FROM Enrollments WHERE student_id = @studentId AND semester = @currentSemester",
                new { studentId, currentSemester }
            );

            Console.WriteLine("Student marked inactive and current semester enrollments removed.");
        }
    }

    public async Task DeleteCourse(int courseId, string currentSemester)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.ExecuteAsync(
                "UPDATE Courses SET IsActive = FALSE WHERE id = @courseId",
                new { courseId }
            );

            await connection.ExecuteAsync(
                "DELETE FROM Enrollments WHERE course_id = @courseId AND semester = @currentSemester",
                new { courseId, currentSemester }
            );

            Console.WriteLine("Course marked inactive and current semester enrollments removed.");
        }
    }

    public async Task GetEnrollments(
        string? studentName,
        string? courseName,
        string? semester,
        bool onlyActive)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            var result = await connection.QueryAsync(
                "SELECT Students.name AS StudentName, Courses.name AS CourseName, Enrollments.semester " +
                "FROM Enrollments " +
                "INNER JOIN Students ON Enrollments.student_id = Students.id " +
                "INNER JOIN Courses ON Enrollments.course_id = Courses.id " +
                "WHERE (@studentName IS NULL OR Students.name = @studentName) " +
                "AND (@courseName IS NULL OR Courses.name = @courseName) " +
                "AND (@semester IS NULL OR Enrollments.semester = @semester) " +
                "AND (@onlyActive = FALSE OR (Students.IsActive = TRUE AND Courses.IsActive = TRUE))",
                new { studentName, courseName, semester, onlyActive }
            );

            foreach (var enrollment in result)
            {
                Console.WriteLine(
                    $"{enrollment.studentname} | {enrollment.coursename} | {enrollment.semester}"
                );
            }
        }
    }
}
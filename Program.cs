// I USED CHATGPT TO WRITE THESE TESTS CASES.

DapperRepository repository = new DapperRepository();

string currentSemester = "2026-S2";
string oldSemester = "2026-S1";


// --------------------------------------------------
// ADD STUDENTS
// --------------------------------------------------

for (int i = 1; i <= 10; i++)
{
    await repository.AddStudent(new Students
    {
        Id = i,
        Name = i == 1 ? "Ahmad" : "Student" + i,
        Email = "student" + i + "@gmail.com",
        IsActive = true
    });
}


// --------------------------------------------------
// ADD COURSES
// --------------------------------------------------

await repository.AddCourse(new Courses
{
    Id = 1,
    Name = "Database Systems",
    IsActive = true
});

await repository.AddCourse(new Courses
{
    Id = 2,
    Name = "C#",
    IsActive = true
});

await repository.AddCourse(new Courses
{
    Id = 3,
    Name = "Networks",
    IsActive = true
});

await repository.AddCourse(new Courses
{
    Id = 4,
    Name = "Algorithms",
    IsActive = true
});


// --------------------------------------------------
// TEST 1
// NORMAL ENROLLMENT
// --------------------------------------------------

Console.WriteLine("\n--- Normal Enrollment ---");

await repository.AddEnrollment(new Enrollments
{
    Id = 1,
    StudentId = 1,
    CourseId = 1,
    Semester = currentSemester
});


// --------------------------------------------------
// TEST 2
// DUPLICATE ENROLLMENT
// --------------------------------------------------

Console.WriteLine("\n--- Duplicate Enrollment ---");

await repository.AddEnrollment(new Enrollments
{
    Id = 2,
    StudentId = 1,
    CourseId = 1,
    Semester = currentSemester
});


// --------------------------------------------------
// TEST 3
// MAXIMUM 3 COURSES
// --------------------------------------------------

Console.WriteLine("\n--- Student 3 Course Limit ---");

await repository.AddEnrollment(new Enrollments
{
    Id = 3,
    StudentId = 1,
    CourseId = 2,
    Semester = currentSemester
});

await repository.AddEnrollment(new Enrollments
{
    Id = 4,
    StudentId = 1,
    CourseId = 3,
    Semester = currentSemester
});


// Ahmad already has 3 courses.
// This should be rejected.

await repository.AddEnrollment(new Enrollments
{
    Id = 5,
    StudentId = 1,
    CourseId = 4,
    Semester = currentSemester
});


// --------------------------------------------------
// TEST 4
// COURSE CAPACITY = 8 STUDENTS
// --------------------------------------------------

Console.WriteLine("\n--- Course Capacity ---");

// Ahmad is already student number 1 in Database Systems.
// Add students 2 through 8.

for (int i = 2; i <= 8; i++)
{
    await repository.AddEnrollment(new Enrollments
    {
        Id = 100 + i,
        StudentId = i,
        CourseId = 1,
        Semester = currentSemester
    });
}


// Student 9 would be the 9th student.
// This should be rejected.

await repository.AddEnrollment(new Enrollments
{
    Id = 109,
    StudentId = 9,
    CourseId = 1,
    Semester = currentSemester
});


// --------------------------------------------------
// TEST 5
// HISTORICAL ENROLLMENT
// --------------------------------------------------

Console.WriteLine("\n--- Previous Semester Enrollment ---");

await repository.AddEnrollment(new Enrollments
{
    Id = 200,
    StudentId = 1,
    CourseId = 4,
    Semester = oldSemester
});


// --------------------------------------------------
// TEST 6
// GENERAL QUERY
// Ahmad's courses in current semester
// --------------------------------------------------

Console.WriteLine("\n--- Ahmad Current Courses ---");

await repository.GetEnrollments(
    "Ahmad",
    null,
    currentSemester,
    false
);


// --------------------------------------------------
// TEST 7
// SOFT DELETE STUDENT
// --------------------------------------------------

Console.WriteLine("\n--- Delete Ahmad ---");

await repository.DeleteStudent(
    1,
    currentSemester
);


// --------------------------------------------------
// TEST 8
// TRY TO ENROLL INACTIVE STUDENT
// --------------------------------------------------

Console.WriteLine("\n--- Inactive Student Test ---");

await repository.AddEnrollment(new Enrollments
{
    Id = 300,
    StudentId = 1,
    CourseId = 4,
    Semester = currentSemester
});


// --------------------------------------------------
// TEST 9
// CHECK OLD ENROLLMENT STILL EXISTS
// --------------------------------------------------

Console.WriteLine("\n--- Ahmad Previous Semester History ---");

await repository.GetEnrollments(
    "Ahmad",
    null,
    oldSemester,
    false
);


// --------------------------------------------------
// TEST 10
// SOFT DELETE COURSE
// --------------------------------------------------

Console.WriteLine("\n--- Delete Database Systems ---");

await repository.DeleteCourse(
    1,
    currentSemester
);


// --------------------------------------------------
// TEST 11
// TRY TO ENROLL INTO INACTIVE COURSE
// --------------------------------------------------

Console.WriteLine("\n--- Inactive Course Test ---");

await repository.AddEnrollment(new Enrollments
{
    Id = 400,
    StudentId = 9,
    CourseId = 1,
    Semester = currentSemester
});


// --------------------------------------------------
// TEST 12
// ALL ACTIVE CURRENT ENROLLMENTS
// --------------------------------------------------

Console.WriteLine("\n--- Active Current Enrollments ---");

await repository.GetEnrollments(
    null,
    null,
    currentSemester,
    true
);


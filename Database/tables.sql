CREATE TABLE Students (
    id INT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);


CREATE TABLE Courses (
    id INT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);


CREATE TABLE Enrollments (
    id INT PRIMARY KEY,
    student_id INT NOT NULL,
    course_id INT NOT NULL,
    semester VARCHAR(255) NOT NULL,

    FOREIGN KEY (student_id) REFERENCES Students(id),
    FOREIGN KEY (course_id) REFERENCES Courses(id),

    UNIQUE (student_id, course_id, semester)
);
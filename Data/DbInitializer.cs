using System;
using System.Collections.Generic;
using System.Text;
using Universities.Models;

namespace Universities.Data
{
    public static class DbInitializer
    {
        public static void Initialize(UnisContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // 1. Создаем университеты
            var harvard = new University { Name = "Гарвард", Address = "Проспект Барнаула 228" };
            var MIT = new University { Name = "МАйТи", Address = "Пендосостан" };

            context.Universitys.AddRange(harvard, MIT);

            // 2. Создаем много групп
            var groups = new List<Group>
            {
                // Группы для MIT
                new Group { Name = "ТР-26", Year = 2026, MaxStudents = 50, Specialty = "Технологии и разработка", University = MIT },
                new Group { Name = "ИИ-25", Year = 2025, MaxStudents = 40, Specialty = "Искусственный интеллект", University = MIT },
                new Group { Name = "КБ-24", Year = 2024, MaxStudents = 45, Specialty = "Кибербезопасность", University = MIT },
                new Group { Name = "БД-26", Year = 2026, MaxStudents = 35, Specialty = "Базы данных и Big Data", University = MIT },
                new Group { Name = "РБ-23", Year = 2023, MaxStudents = 30, Specialty = "Робототехника", University = MIT },

                // Группы для Гарварда
                new Group { Name = "Д-24", Year = 2024, MaxStudents = 30, Specialty = "Дизайн", University = harvard },
                new Group { Name = "ЮР-23", Year = 2023, MaxStudents = 60, Specialty = "Юриспруденция", University = harvard },
                new Group { Name = "ЭК-25", Year = 2025, MaxStudents = 55, Specialty = "Экономика и менеджмент", University = harvard },
                new Group { Name = "ПС-26", Year = 2026, MaxStudents = 40, Specialty = "Психология", University = harvard },
                new Group { Name = "МД-24", Year = 2024, MaxStudents = 25, Specialty = "Медиа и журналистика", University = harvard }
            };

            context.Groups.AddRange(groups);

            // 3. Генерируем базу студентов (Имена, Фамилии и случайное распределение)
            var firstNames = new[] {
                "John", "Jane", "Alex", "Emily", "Michael", "Sarah", "David", "Jessica", "James", "Emma",
                "Robert", "Ashley", "William", "Amanda", "Brian", "Megan", "Kevin", "Rachel", "Jason", "Stephanie",
                "Daniel", "Nicole", "Matthew", "Christina", "Andrew", "Lisa", "Joshua", "Rebecca", "Christopher", "Amanda",
                "Ryan", "Amber", "Ethan", "Sophia", "Jacob", "Olivia", "Liam", "Ava", "Noah", "Isabella"
            };

            var lastNames = new[] {
                "Rampage", "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez",
                "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson",
                "Martin", "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis",
                "Robinson", "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill"
            };

            var students = new List<Student>();
            var random = new Random(42); // Seed для воспроизводимости дат и распределения

            // Генерируем 60 уникальных студентов и раскидываем их по созданным группам
            for (int i = 0; i < 60; i++)
            {
                // Берем случайное имя и фамилию из массивов
                string fName = firstNames[random.Next(firstNames.Length)];
                string lName = lastNames[random.Next(lastNames.Length)];

                // Выбираем случайную группу из списка groups
                Group assignedGroup = groups[random.Next(groups.Count)];

                // Генерируем случайную дату рождения (от 1995 до 2007 года)
                int year = random.Next(1995, 2008);
                int month = random.Next(1, 13);
                int day = random.Next(1, 29); // 28 дней, чтобы не упасть на феврале
                DateTime birthDate = new DateTime(year, month, day);

                students.Add(new Student
                {
                    FirstName = fName,
                    LastName = lName,
                    Group = assignedGroup,
                    BirthDate = birthDate
                });
            }

            context.Students.AddRange(students);

            // 4. Сохраняем все в базу данных одной транзакцией
            context.SaveChanges();
        }
    }
}
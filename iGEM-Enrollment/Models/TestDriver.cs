using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace iGEM_Enrollment.Models
{
    public class TestDriver
    {
        public static void TestDBInit(ApplyContext context)
        {
            context.Database.EnsureCreated();

            if (context.AppliForm.Any())
            {
                return;
            }

            var appliForm = new AppliForm[]
            {
                new AppliForm
                {
                    appliFormId = "0",
                    stuUnionText = "Test1",
                    isResearch = true,
                    introText = "Test1",
                    prizeText = "Test1",
                    researchText = "Test1"
                },
                new AppliForm
                {
                    appliFormId = "1",
                    stuUnionText = "Test1",
                    isResearch = true,
                    introText = "Test1",
                    prizeText = "Test1",
                    researchText = "Test1"
                },
                new AppliForm
                {
                    appliFormId = "2",
                    stuUnionText = "Test1",
                    isResearch = true,
                    introText = "Test1",
                    prizeText = "Test1",
                    researchText = "Test1"
                },
                new AppliForm
                {
                    appliFormId = "3",
                    stuUnionText = "Test1",
                    isResearch = true,
                    introText = "Test1",
                    prizeText = "Test1",
                    researchText = "Test1"
                },
                new AppliForm
                {
                    appliFormId = "4",
                    stuUnionText = "Test1",
                    isResearch = true,
                    introText = "Test1",
                    prizeText = "Test1",
                    researchText = "Test1"
                },
                new AppliForm
                {
                    appliFormId = "5",
                    stuUnionText = "Test1",
                    isResearch = true,
                    introText = "Test1",
                    prizeText = "Test1",
                    researchText = "Test1"
                },
                new AppliForm
                {
                    appliFormId = "6",
                    stuUnionText = "Test1",
                    isResearch = true,
                    introText = "Test1",
                    prizeText = "Test1",
                    researchText = "Test1"
                },
                new AppliForm
                {
                    appliFormId = "7",
                    stuUnionText = "Test1",
                    isResearch = true,
                    introText = "Test1",
                    prizeText = "Test1",
                    researchText = "Test1"
                }
            };

            foreach (AppliForm a in appliForm)
            {
                context.AppliForm.Add(a);
            }
            context.SaveChanges();

            var applicant = new Applicant[]
            {
                new Applicant
                {
                    stuId = 2015141244000,
                    appliForm = appliForm[0],
                    birthDate = DateTime.Parse("1996-11"),
                    college = "LSC",
                    email = "a@b.com",
                    engGrade = 80,
                    engType = EnglishTypeEnum.CET4,
                    gender = GenderEnum.Female,
                    grade = 2015,
                    major = "CB",
                    name = "TestName",
                    phone = "17761298473",
                    stuFrom = "Beijing"
                }
            };
            foreach (Applicant a in applicant)
            {
                context.Applicant.Add(a);
            }
            context.SaveChanges();

            var users = new User[]
            {
                new User {name = "程昊阳", stuId = 2015141244003, token = ""},
                new User {name = "张小祺", stuId = 2015141244026, token = ""}
            };
            foreach (User userData in users)
            {
                context.UserData.Add(userData);
            }
            context.SaveChanges();

        }

        public static void TestEncryption()
        {
            var passList = new string[100];

            for (int i = 0; i < 100; i++)
            {
                passList[i] = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: "testPassword",
                    salt: System.Text.Encoding.UTF8.GetBytes("123456"),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));
            }
        }
    }
}

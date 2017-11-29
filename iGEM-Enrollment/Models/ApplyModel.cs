using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGEM_Enrollment
{
    public enum GenderEnum { Male, Female }
    public enum EnglishTypeEnum { HighSchool, CET4, CET6, ToeIls }

    [Serializable]
    public class FormString
    {
        public String stuId { get; set; }
        public String name { get; set; }
        public String birthDate { get; set; }
        public String phone { get; set; }
        public String gender { get; set; }
        public String email { get; set; }
        public String grade { get; set; }
        public String college { get; set; }
        public String major { get; set; }
        public String stuFrom { get; set; }
        public String engType { get; set; }
        public String engGrade { get; set; }
        public String stuUnionText { get; set; }
        public String isResearch { get; set; }
        public String researchText { get; set; }
        public String prizeText { get; set; }
        public String introText { get; set; }
        public String appendixFileName { get; set; }
        public String photoFileName { get; set; }

        public FormString() { }

        public FormString(Applicant a)
        {
            stuId = a.stuId.ToString();
            name = a.name;
            birthDate = a.birthDate.ToString("yyyy-MM");
            phone = a.phone;
            gender = a.gender == GenderEnum.Male ? "M" : "F";
            email = a.email;
            grade = a.grade.ToString();
            college = a.college;
            major = a.major;
            stuFrom = a.stuFrom;
            switch (a.engType)
            {
                case EnglishTypeEnum.ToeIls:
                    engType = "toeils";
                    break;
                case EnglishTypeEnum.CET6:
                    engType = "cet6";
                    break;
                case EnglishTypeEnum.CET4:
                    engType = "cet4";
                    break;
                default:
                    engType = "highSchool";
                    break;
            }
            engGrade = a.engGrade.ToString();
            stuUnionText = a.appliForm.stuUnionText;
            isResearch = a.appliForm.isResearch ? "Yes" : "No";
            researchText = a.appliForm.researchText;
            prizeText = a.appliForm.prizeText;
            introText = a.appliForm.introText;
            appendixFileName = a.appliForm.appendixFileName;
            photoFileName = a.photoFileName;
        }
    }

    public class SavedForm
    {
        public DateTime savedTime { get; set; }
        public FormString theForm { get; set; }

        [Key]
        public String token { get; set; }
    }

    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        [Key]
        public long stuId { get; set; }
        public String name { get; set; }
        public String token { get; set; }
    }

    public class HoloForm
    {

        public long stuId { get; set; }
        public String name { get; set; }
        public DateTime birthDate { get; set; }
        public String phone { get; set; }
        public GenderEnum gender { get; set; }
        public String email { get; set; }
        public int grade { get; set; }
        public String college { get; set; }
        public String major { get; set; }
        public String stuFrom { get; set; }
        public EnglishTypeEnum engType { get; set; }
        public float engGrade { get; set; }
        public String stuUnionText { get; set; }
        public bool isResearch { get; set; }
        public String researchText { get; set; }
        public String prizeText { get; set; }
        public String introText { get; set; }
        public String appendixFileName { get; set; }
        public String photoFileName { get; set; }

        public HoloForm(FormString formString)
        {
            stuId = long.Parse(formString.stuId);
            name = formString.name;
            birthDate = DateTime.Parse(formString.birthDate);
            phone = formString.phone;
            gender = formString.gender == "M" ? GenderEnum.Male : GenderEnum.Female;
            email = formString.email;
            grade = int.Parse(formString.grade);
            college = formString.college;
            major = formString.major;
            stuFrom = formString.stuFrom;

            String inputEngType = formString.engType;

            if (inputEngType == "highSchool")
            {
                engType = EnglishTypeEnum.HighSchool;
            }
            else if (inputEngType == "cet4")
            {
                engType = EnglishTypeEnum.CET4;
            }
            else if (inputEngType == "cet6")
            {
                engType = EnglishTypeEnum.CET6;
            }
            else
            {
                engType = EnglishTypeEnum.ToeIls;
            }

            engGrade = float.Parse(formString.engGrade);
            stuUnionText = formString.stuUnionText;
            isResearch = formString.isResearch == "Yes";
            researchText = formString.researchText;
            prizeText = formString.prizeText;
            introText = formString.introText;
            photoFileName = formString.photoFileName;
            appendixFileName = formString.appendixFileName;
        }

        public String ToString()
        {
            return stuId + "__" + name + "__" + birthDate + "__" + phone + "__" + gender + "__" + email + "__" + grade + "__" + college + "__" + major + "__" + stuFrom + "__" + engType + "__" + engGrade + "__" + stuUnionText + "__" + isResearch + "__" + researchText + "__" + prizeText + "__" + introText + "__" + photoFileName + "__" + appendixFileName;
        }
    }

    public class Applicant
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        [Key]
        public long stuId { get; set; }
        public String name { get; set; }
        public DateTime birthDate { get; set; }
        public String phone { get; set; }
        public GenderEnum gender { get; set; }
        public String email { get; set; }
        public int grade { get; set; }
        public String college { get; set; }
        public String major { get; set; }
        public String stuFrom { get; set; }
        public EnglishTypeEnum engType { get; set; }
        public float engGrade { get; set; }
        public String photoFileName { get; set; }

        public AppliForm appliForm { get; set; }


        public Applicant() { }

        public Applicant(HoloForm hf, AppliForm af)
        {
            stuId = hf.stuId;
            name = hf.name;
            birthDate = hf.birthDate;
            phone = hf.phone;
            gender = hf.gender;
            email = hf.email;
            grade = hf.grade;
            college = hf.college;
            major = hf.major;
            stuFrom = hf.stuFrom;
            engType = hf.engType;
            engGrade = hf.engGrade;
            photoFileName = hf.photoFileName;

            appliForm = af;
        }
    }

    public class AppliForm
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        [Key]
        public String appliFormId { get; set; }
        public String stuUnionText { get; set; }
        public bool isResearch { get; set; }
        public String researchText { get; set; }
        public String prizeText { get; set; }
        public String introText { get; set; }
        public String appendixFileName { get; set; }


        public AppliForm() { }

        public AppliForm(HoloForm hf)
        {
            appliFormId = hf.stuId + "_" + DateTime.Now.ToString("o");
            stuUnionText = hf.stuUnionText;
            isResearch = hf.isResearch;
            researchText = hf.researchText;
            prizeText = hf.prizeText;
            introText = hf.introText;
            appendixFileName = hf.appendixFileName;
        }
    }

    public class Interview
    {
        public String stuId;
        public bool time1;
    }
}

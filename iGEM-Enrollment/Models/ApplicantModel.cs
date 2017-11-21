using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization.Json;
using System.Linq;
using System.Threading.Tasks;

namespace iGEM_Enrollment
{
    [Serializable]
    public class ApplicantString
    {
        [Display(Name = "StuID")]
        public String stuId { get; set; }

        [Display(Name = "Name")]
        public String name { get; set; }

        [Display(Name = "BirthDate")]
        public String birthDate { get; set; }

        [Display(Name = "Phone")]
        public String phone { get; set; }

        [Display(Name = "Gender")]
        public String gender { get; set; }

        [Display(Name = "Email")]
        public String email { get; set; }

        [Display(Name = "Grade")]
        public String grade { get; set; }

        [Display(Name = "College")]
        public String college { get; set; }

        [Display(Name = "Major")]
        public String major { get; set; }

        [Display(Name = "From")]
        public String stuFrom { get; set; }

        [Display(Name = "EnglishType")]
        public String engType { get; set; }

        [Display(Name = "EnglishGrade")]
        public String engGrade { get; set; }

        [Display(Name = "StuUnion")]
        public String stuUnionText { get; set; }

        [Display(Name = "IsResearch")]
        public String isResearch { get; set; }

        [Display(Name = "researchText")]
        public String researchText { get; set; }

        [Display(Name = "Prizes")]
        public String prizeText { get; set; }

        [Display(Name = "Intro")]
        public String introText { get; set; }
    }

    [Serializable]
    public class InitDataString
    {
        [Display(Name = "Name")] public String name;
        [Display(Name = "StuId")] public String stuId;
    }

    public class InitData
    {
        public String name;
        public int stuId;
    }

    public class Applicant
    {
        public enum GenderEnum { Male, Female }
        public enum EnglishTypeEnum { HighSchool, CET4, CET6, ToeIls }

        public int stuId { get; set; }
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
        public int engGrade { get; set; }
        public String stuUnionText { get; set; }
        public bool isResearch { get; set; }
        public String researchText { get; set; }
        public String prizeText { get; set; }
        public String introText { get; set; }
    }

}

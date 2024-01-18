namespace Customer_Relationship_Managament.DTO
{
    public class ReadCCCD_DTO
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public List<Datum> data { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }
        public string id_prob { get; set; }
        public string name { get; set; }
        public string name_prob { get; set; }
        public string dob { get; set; }
        public string dob_prob { get; set; }
        public string sex { get; set; }
        public string sex_prob { get; set; }
        public string nationality { get; set; }
        public string nationality_prob { get; set; }
        public string home { get; set; }
        public string home_prob { get; set; }
        public string address { get; set; }
        public string address_prob { get; set; }
        public string doe { get; set; }
        public string doe_prob { get; set; }
        public string overall_score { get; set; }
        public string number_of_name_lines { get; set; }
        public Address_Entities address_entities { get; set; }
        public string type_new { get; set; }
        public string type { get; set; }
    }

    public class Address_Entities
    {
        public string province { get; set; }
        public string district { get; set; }
        public string ward { get; set; }
        public string street { get; set; }
    }
}
 
using System.Collections.Generic;

namespace Uranus.Domain.Entities
{
    public class Lawsuits
    {
        public List<Customers> Customers { get; set; }

        public List<PartsLawyers> PartsLawyers { get; set; }

        public List<Actions> Actions { get; set; }

        public List<Events> Events { get; set; }

        public List<ScheduleProfessional> ScheduleProfessional { get; set; }
    }

    public class Customers
    {
        public int IdCustomer { get; set; }
        public string Customer { get; set; }
    }

    public class PartsLawyers
    {
        public int IdPart { get; set; }
        public string Part { get; set; }
        public string Lawyer { get; set; }
    }

    public class Actions
    {
        public int IdAction { get; set; }
        public int IdStick { get; set; }
        public int IdArea { get; set; }
    }

    public class Events
    {
        public int IdAction { get; set; }
        public int IdEvent { get; set; }
    }

    public class ScheduleProfessional
    {
        public int IdAction { get; set; }
    }
}
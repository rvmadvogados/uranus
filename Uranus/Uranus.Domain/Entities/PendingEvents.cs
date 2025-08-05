using System.Collections.Generic;

namespace Uranus.Domain.Entities
{
    public class PendingEvents
    {
        public List<CaptureEProc> CaptureEProc { get; set; }

        public List<CaptureWebJur> CaptureWebJur { get; set; }

        public List<CaptureOAB> CaptureOAB { get; set; }
    }

    public class CaptureEProc
    {
        public int Id { get; set; }
        public string PublicationDate { get; set; }
        public string ProcessNumber { get; set; }
        public string Client { get; set; }
        public string Organ { get; set; }
        public string Class { get; set; }
        public string Subject { get; set; }
        public string IdProcessAction { get; set; }
        public bool HasProcess { get; set; }

    }

    public class CaptureWebJur
    {
        public int Id { get; set; }
        public string PublicationDate { get; set; }
        public string ProcessNumber { get; set; }
        public string Client { get; set; }
        public string Organ { get; set; }
        public string Stick { get; set; }
        public string PublicationCode { get; set; }
        public string IdProcessAction { get; set; }
        public bool HasProcess { get; set; }

    }

    public class CaptureOAB
    {
        public int Id { get; set; }
        public string PublicationDate { get; set; }
        public string ProcessNumber { get; set; }
        public string County { get; set; }
        public string Client { get; set; }
        public string Stick { get; set; }
        public string IdProcessAction { get; set; }
        public bool HasProcess { get; set; }

    }
}
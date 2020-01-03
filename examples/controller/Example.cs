namespace Engineerd.KubeController
{
    public class ExampleSpec
    {
        public string ExampleType { get; set; }
        public string ProgrammingLanguage { get; set; }
    }

    public class ExampleStatus
    {
        public int Count { get; set; }
    }

    public class ExampleCRD : CustomResource<ExampleSpec, ExampleStatus>
    {
    }
}

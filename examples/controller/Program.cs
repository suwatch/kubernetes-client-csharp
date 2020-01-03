using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using k8s;

namespace Engineerd.KubeController
{
    public static class Program
    {
        public static async Task Main()
        {
            try
            {
                var envs = Environment.GetEnvironmentVariables();
                foreach (var key in envs.Keys)
                {
                    Console.WriteLine($"{key}: {envs[key]}");
                }

                var config = KubernetesClientConfiguration.BuildDefaultConfig();
                Console.WriteLine($"config.Host: {config.Host}");
                Console.WriteLine($"config.AccessToken: {config.AccessToken}");
                Console.WriteLine($"config.SslCaCerts: {config.SslCaCerts}");

                IKubernetes client = new Kubernetes(config);
                Console.WriteLine("Starting Request!");

                var list = client.ListNamespacedPod("default");
                foreach (var item in list.Items)
                {
                    Console.WriteLine(item.Metadata.Name);
                }
                if (list.Items.Count == 0)
                {
                    Console.WriteLine("Empty!");
                }


                await Run(config);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                Thread.Sleep(30000);
            }
        }

        public static async Task Run(KubernetesClientConfiguration config)
        {
            var crd = new CustomResourceDefinition()
            {
                ApiVersion = "engineerd.dev/v1alpha1",
                PluralName = "examples",
                Kind = "Example",
                Namespace = "default"
            };

            var controller = new Controller<ExampleCRD>(
                new Kubernetes(config),
                crd,
                (WatchEventType eventType, ExampleCRD example) =>
                {
                    Console.WriteLine("Event type: {0} for {1}", eventType, example.Metadata.Name);
                });

            var cts = new CancellationTokenSource();
            await controller.StartAsync(cts.Token).ConfigureAwait(false);
        }
    }
}

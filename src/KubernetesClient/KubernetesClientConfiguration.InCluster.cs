using System;
using System.IO;
using k8s.Exceptions;

namespace k8s
{
    public partial class KubernetesClientConfiguration
    {
        //private static string ServiceAccountPath =
        //    Path.Combine(new string[] {
        //        "var", "run", "secrets", "kubernetes.io", "serviceaccount/"
        //    });
        //private const string ServiceAccountTokenKeyFileName = "token";
        //private const string ServiceAccountRootCAKeyFileName = "ca.crt";

        private const string ServiceAccountTokenKeyPath = "/var/run/secrets/kubernetes.io/serviceaccount/token";
        private const string ServiceAccountRootCAKeyPath = "/var/run/secrets/kubernetes.io/serviceaccount/ca.crt";

        public static Boolean IsInCluster()
        {
            var host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
            var port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port))
            {
                return false;
            }

            //var tokenPath = Path.DirectorySeparatorChar + Path.Combine(ServiceAccountPath, ServiceAccountTokenKeyFileName);
            //Console.WriteLine($"File.Exists({tokenPath}): {File.Exists(tokenPath)}");
            if (!File.Exists(ServiceAccountTokenKeyPath))
            {
                return false;
            }

            //var certPath = Path.DirectorySeparatorChar + Path.Combine(ServiceAccountPath, ServiceAccountRootCAKeyFileName);
            //Console.WriteLine($"File.Exists({certPath}): {File.Exists(certPath)}");
            return File.Exists(ServiceAccountRootCAKeyPath);
        }

        public static KubernetesClientConfiguration InClusterConfig()
        {
            if (!IsInCluster()) {            
                throw new KubeConfigException(
                    "unable to load in-cluster configuration, KUBERNETES_SERVICE_HOST and KUBERNETES_SERVICE_PORT must be defined");
            }

            //var token = File.ReadAllText(Path.Combine(ServiceAccountPath, ServiceAccountTokenKeyFileName));
            var token = File.ReadAllText(ServiceAccountTokenKeyPath);
            //var rootCAFile = Path.Combine(ServiceAccountPath, ServiceAccountRootCAKeyFileName);
            var rootCAFile = ServiceAccountRootCAKeyPath;
            var host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
            var port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");

            return new KubernetesClientConfiguration
            {
                Host = new UriBuilder("https", host, Convert.ToInt32(port)).ToString(),
                AccessToken = token,
                SslCaCerts = CertUtils.LoadPemFileCert(rootCAFile)
            };
        }
    }
}

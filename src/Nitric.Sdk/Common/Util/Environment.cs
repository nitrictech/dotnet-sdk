using SystemEnv = System.Environment;

namespace Nitric.Sdk.Common.Util
{
    /// <summary>
    /// Helper Utils for access common Nitric environment variables
    /// </summary>
    internal static class Environment
    {
        private const string NitricHostAddressDefault = "localhost:50051";
        private const string NitricHostAddressEnvVar = "SERVICE_ADDRESS";

        private static string GetEnvironmentVariable(string name, string defaultValue)
            => SystemEnv.GetEnvironmentVariable(name) is { Length: > 0 } v ? v : defaultValue;

        internal static string GetNitricHost() =>
            GetEnvironmentVariable(NitricHostAddressEnvVar, NitricHostAddressDefault);
    }
}

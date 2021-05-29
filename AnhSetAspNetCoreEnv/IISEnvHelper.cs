using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;

namespace AnhSetAspNetCoreEnv
{
    class IISEnvHelper
    {
        public static void AddEnv(string envName,string envValue)
        {

            using (ServerManager serverManager = new ServerManager())
            {
                Configuration config = serverManager.GetApplicationHostConfiguration();

                ConfigurationSection aspNetCoreSection = config.GetSection("system.webServer/aspNetCore");

                ConfigurationElementCollection environmentVariablesCollection = aspNetCoreSection.GetCollection("environmentVariables");

                ConfigurationElement environmentVariableElement = environmentVariablesCollection.CreateElement("environmentVariable");
                
                //environmentVariableElement["name"] = @"ASPNETCORE_ENVIRONMENT";
                //environmentVariableElement["value"] = @"Production";

                environmentVariableElement["name"] = envName;
                environmentVariableElement["value"] = envValue;

                environmentVariablesCollection.Add(environmentVariableElement);

                serverManager.CommitChanges();
            }
        }

        public static string GetEnvValue(string envName)
        {
            var result = $"未读取到 环境变量：“{envName}” 的值。";
            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Configuration config = serverManager.GetApplicationHostConfiguration();

                    ConfigurationSection aspNetCoreSection = config.GetSection("system.webServer/aspNetCore");

                    ConfigurationElementCollection environmentVariablesCollection = aspNetCoreSection.GetCollection("environmentVariables");

                    //ConfigurationElement environmentVariableElement = FindElement(environmentVariablesCollection, "environmentVariable", "name", @"ASPNETCORE_ENVIRONMENT", "value", @"Production");
                    ConfigurationElement environmentVariableElement = FindElement(environmentVariablesCollection, "environmentVariable", "name", envName);

                    result = $"{environmentVariableElement.GetAttributeValue("value")}";
                }
            }
            catch (Exception ex)
            {
                result += $"{ex.Message}";
            }
            return result;
        }

        public static string AppVersion()
        {
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(IISEnvHelper));
            var assemblyName = assembly.GetName();

            var result = $"{(assemblyName.Version==null?"1.0":$"{ assemblyName.Version}")}";
            return result;
        }

        public static void SetEnv(string envName, string envValue)
        {
            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Configuration config = serverManager.GetApplicationHostConfiguration();

                    ConfigurationSection aspNetCoreSection = config.GetSection("system.webServer/aspNetCore");

                    ConfigurationElementCollection environmentVariablesCollection = aspNetCoreSection.GetCollection("environmentVariables");

                    //ConfigurationElement environmentVariableElement = FindElement(environmentVariablesCollection, "environmentVariable", "name", @"ASPNETCORE_ENVIRONMENT");
                    ConfigurationElement environmentVariableElement = FindElement(environmentVariablesCollection, "environmentVariable", "name", envName);
                    if (environmentVariableElement == null)
                    {
                        throw new InvalidOperationException("Element not found!");
                    }

                    //environmentVariableElement["value"] = @"Development";
                    environmentVariableElement["value"] = envValue;

                    serverManager.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                AddEnv(envName, envValue);
            }
        }

        public static void DelEnv(string envName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                Configuration config = serverManager.GetApplicationHostConfiguration();

                ConfigurationSection aspNetCoreSection = config.GetSection("system.webServer/aspNetCore");

                ConfigurationElementCollection environmentVariablesCollection = aspNetCoreSection.GetCollection("environmentVariables");

                //ConfigurationElement environmentVariableElement = FindElement(environmentVariablesCollection, "environmentVariable", "name", @"ASPNETCORE_ENVIRONMENT", "value", @"Production");
                ConfigurationElement environmentVariableElement = FindElement(environmentVariablesCollection, "environmentVariable", "name", envName);
                if (environmentVariableElement == null)
                {
                    //throw new InvalidOperationException("Element not found!");
                    Console.WriteLine($"Element “system.webServer/aspNetCore:environmentVariables/environmentVariable:{envName}” not found!");
                }

                environmentVariablesCollection.Remove(environmentVariableElement);

                serverManager.CommitChanges();
            }
        }

        public static ConfigurationElement FindElement(ConfigurationElementCollection collection, string elementTagName, params string[] keyValues)
        {
            foreach (ConfigurationElement element in collection)
            {
                if (String.Equals(element.ElementTagName, elementTagName, StringComparison.OrdinalIgnoreCase))
                {
                    bool matches = true;

                    for (int i = 0; i < keyValues.Length; i += 2)
                    {
                        object o = element.GetAttributeValue(keyValues[i]);
                        string value = null;
                        if (o != null)
                        {
                            value = o.ToString();
                        }

                        if (!String.Equals(value, keyValues[i + 1], StringComparison.OrdinalIgnoreCase))
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (matches)
                    {
                        return element;
                    }
                }
            }
            return null;
        }

    }
}

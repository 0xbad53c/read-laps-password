using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;

namespace read_laps_pwd
{
    class Program
    {
        static void Main(string[] args)
        {
            string computerHostName = args[0]; // target host to read LAPS pwd from
            string domain = args[1]; // AD domain to query

            DirectoryContext dirCtx = new DirectoryContext(DirectoryContextType.Domain, domain);
            using (Domain compsDomain = Domain.GetDomain(dirCtx))
            using (DirectorySearcher adSearcher = new DirectorySearcher(compsDomain.GetDirectoryEntry()))
            {
                //this is the search criteria for the domain query
                adSearcher.Filter = "(&(objectClass=computer) (cn=" + computerHostName + "))";
                adSearcher.SearchScope = SearchScope.Subtree;
                adSearcher.PropertiesToLoad.Add("ms-Mcs-AdmPwd");
                adSearcher.PropertiesToLoad.Add("ms-Mcs-AdmPwdExpirationTime");
                SearchResult searchResult = adSearcher.FindOne();

                //Get the LAPS password
                Console.WriteLine(searchResult.GetDirectoryEntry().Properties["ms-Mcs-AdmPwd"].Value);
                //Should get the LAPS password expiration time
                Console.WriteLine((long)searchResult.Properties["ms-Mcs-AdmPwdExpirationTime"][0]);
            }
        }
    }
}

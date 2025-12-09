using Crolow.Cms.Server.Core.Interfaces.Models;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using MongoDB.Bson.Serialization;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Kalow.Apps.Managers.Mongo.Utils.Mapping
{
    public class AutoMapper
    {
        public static void DefaultMappers(Assembly asm)
        {
            foreach (Type t in asm.GetTypes())
            {
                if (!t.IsInterface)
                {
                    if (t.GetInterface(typeof(IDataObject).Name) != null)
                    {
                        BsonClassMap.LookupClassMap(t);
                        continue;
                    }

                    if (t.GetInterface(typeof(IDataMapped).Name) != null)
                    {
                        BsonClassMap.LookupClassMap(t);
                        continue;
                    }
                }
            }
        }

        public static void DefaultMappers(string[] patterns)
        {
            IEnumerator enumerator = Thread.GetDomain().GetAssemblies().GetEnumerator();
            while (enumerator.MoveNext())
            {
                try
                {
                    var a = (Assembly)enumerator.Current;

                    string aName = a.FullName;
                    if (patterns.Count(p => p == aName.Substring(0, p.Length)) > 0)
                    {
                        DefaultMappers(a);
                    }
                }
                catch
                {
                    ;
                }
            }
        }

    }
}

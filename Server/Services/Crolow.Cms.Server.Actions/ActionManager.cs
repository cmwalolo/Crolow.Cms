using Crolow.Apps.Common.Reflection;
using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Interfaces.Application;
using Crolow.Cms.Server.Core.Models.Actions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crolow.Cms.Server.Actions
{
    public class ActionManager : IActionManager
    {
        protected static object _lock = new object();
        static IEnumerable<IGrouping<string, Type>> actionDictionary = null;
        IServiceProvider serviceProvider;

        public ActionManager(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        protected List<Type> GetActions(string name)
        {
            if (actionDictionary == null)
            {
                // We lock only if there is actually no dictionary loaded
                lock (_lock)
                {
                    // If previous lock filled in data.. We do not process
                    if (actionDictionary == null)
                    {
                        var actionTypes = ReflectionHelper.GetClassesWithAttribute(typeof(ActionComponentAttribute), true);

                        actionDictionary = actionTypes
                            .OrderBy(p => ((ActionComponentAttribute)p.GetCustomAttributes(typeof(ActionComponentAttribute), false).FirstOrDefault()).ExecutionOrder)
                            .GroupBy(p => ((ActionComponentAttribute)p.GetCustomAttributes(typeof(ActionComponentAttribute), false).FirstOrDefault()).Path);

                    }
                }
            }

            return actionDictionary.FirstOrDefault(p => p.Key == name).ToList();

        }

        public void ProcessAction(string method, BaseRequest request)
        {
            try
            {
                foreach (var action in GetActions(method))
                {
                    var t = (IAction)serviceProvider.GetService(action);

                    t.Process(request);
                    if (request.CancelRequest)
                    {
                        break;
                    }
                }
            }
            finally
            {
            }
        }
    }
}

using JricaStudioApp.Models.Dtos;
using Models.Dtos;
using System.Reflection;

namespace JricaStudioApp.Server.Extentions
{
    static public class DtoDateTimeSanitization
    {
        public static T ConvertDateTimesToUtcTime<T>(this T obj) where T : new()
        {
            T newObj = new T();

            Type dtoType = typeof(T);
            PropertyInfo[] properties = dtoType.GetProperties();

            foreach (PropertyInfo propertyInfo in properties)
            {
                Type propType = propertyInfo.PropertyType;

                if (propType == typeof(DateTime) || propType == typeof(DateTime?))
                {
                    DateTime? localTime;

                    try
                    {
                        localTime = (DateTime)propertyInfo.GetValue(obj);
                    }
                    catch (NullReferenceException e)
                    {
                        continue;
                    }


                    if (localTime.HasValue)
                    {
                        DateTime timeValue = localTime.Value;

                        if (timeValue.Kind == DateTimeKind.Local || timeValue.Kind == DateTimeKind.Unspecified)
                        {
                            DateTime utcTime = timeValue.ToUniversalTime();
                            propertyInfo.SetValue(newObj, utcTime);
                        }
                        else
                        {
                            propertyInfo.SetValue(newObj, timeValue);
                        }
                    }
                    else
                    {
                        propertyInfo.SetValue(newObj, propertyInfo.GetValue(obj));
                    }
                }
                else
                {
                    propertyInfo.SetValue(newObj, propertyInfo.GetValue(obj));
                }
            }
            return newObj;
        }

        public static T ConvertDateTimesToLocalTime<T>(this T obj) where T : new()
        {
            T newObj = new T();

            Type dtoType = typeof(T);
            PropertyInfo[] properties = dtoType.GetProperties();

            foreach (PropertyInfo propertyInfo in properties)
            {
                Type propType = propertyInfo.PropertyType;

                if (propType == typeof(DateTime) || propType == typeof(DateTime?))
                {
                    DateTime? UtcTime;

                    try
                    {
                        UtcTime = (DateTime)propertyInfo.GetValue(obj);
                    }
                    catch (NullReferenceException e)
                    {
                        continue;
                    }

                    if (UtcTime.HasValue)
                    {
                        DateTime timeValue = UtcTime.Value;

                        if (timeValue.Kind == DateTimeKind.Utc || timeValue.Kind == DateTimeKind.Unspecified)
                        {
                            DateTime localTime = timeValue.ToLocalTime();
                            propertyInfo.SetValue(newObj, localTime);
                        }
                        else
                        {
                            propertyInfo.SetValue(newObj, timeValue);
                        }
                    }
                    else
                    {
                        propertyInfo.SetValue(newObj, propertyInfo.GetValue(obj));
                    }
                }
                else
                {
                    propertyInfo.SetValue(newObj, propertyInfo.GetValue(obj));
                }
            }
            return newObj;
        }

        public static IEnumerable<T> ConvertCollectionsDateTimesToLocalTime<T>(this IEnumerable<T> collectionOfObj) where T : new()
        {
            var newCollection = new List<T>();

            int length = collectionOfObj.Count();

            for (int i = 0; i < length; i++)
            {
                var element = collectionOfObj.ElementAt(i);

                newCollection.Add(element.ConvertDateTimesToLocalTime());
            }

            return newCollection;
        }


    }
}

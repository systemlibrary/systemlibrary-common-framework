﻿using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Net.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class IServiceCollectionExtensionsTests : BaseTest
{
    [TestMethod]
    public void AddCommonServices_Does_Not_Throw_And_Adds_At_Least_One_Service()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddCommonServices();

        var type = typeof(IHttpContextAccessor);

        var has = serviceCollection.Any(sd => sd.ServiceType == type);
          
        Assert.IsTrue(has);
    }

//    [TestMethod]
//    public void Do_Not_Create_Data_Protection_File()
//    {
//        var options = new CommonServicesOptions();

//        options.AutoGenerateDataProtectionKeyFile = false;

//        var service = new ServiceCollectionTest();

//        service.AutoGenerateDataProtectionKeyFile(options);
//    }
}


//public class ServiceCollectionTest : IServiceCollection
//{
//    public ServiceDescriptor this[int index] { get { return null; } set { } }

//    public int Count { get; }
//    public bool IsReadOnly { get; }

//    public void Add(ServiceDescriptor item)
//    {
//    }

//    public void Clear()
//    {
//    }

//    public bool Contains(ServiceDescriptor item)
//    {
//        return false;
//    }

//    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
//    {
//    }

//    public IEnumerator<ServiceDescriptor> GetEnumerator()
//    {
//        return null;
//    }

//    public int IndexOf(ServiceDescriptor item)
//    {
//        return 0;
//    }

//    public void Insert(int index, ServiceDescriptor item)
//    {
//    }

//    public bool Remove(ServiceDescriptor item)
//    {
//        return true;
//    }

//    public void RemoveAt(int index)
//    {
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        throw new System.NotImplementedException();
//    }
//}
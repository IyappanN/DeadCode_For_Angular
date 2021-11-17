using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace DeadCodeFinderForAngularTests
{
    public class MethodFinderTests : MethodFinderTestsBase
    {


        // [Fact]
        // [Trait("Requirement", "Get dead code config")]
        // public void GetDeadCodeConfig_Success()
        // {
        //     GetConfiguration();
        //     MethodFinder.GetExecutingDirectoryName();
        //     MethodFinder.GetDeadCodeConfig();
        //     Assert.True(true);
        // }


        [Fact]
        [Trait("Requirement", "Get component List")]
        public void componentMethodList_ArgumentNullException()
        {
            GetConfiguration();
            MethodFinder.projectPath = "";
            Assert.Throws<ArgumentException>(() => MethodFinder.componentMethodList());
        }



        [Fact]
        [Trait("Requirement", "Get method list")]
        public void GetMethodList__ArgumentNullException()
        {
            GetConfiguration();
            MethodFinder.projectPath = "";
            Assert.Throws<ArgumentException>(() => MethodFinder.getMethodList(MethodFinder.projectPath));
        }

        [Fact]
        [Trait("Requirement", "Get Executing DirectoryName")]
        public void GetExecutingDirectoryName()
        {
            GetConfiguration();
            MethodFinder.GetExecutingDirectoryName();

            Assert.True(true);

        }


        [Fact]
        [Trait("Requirement", "Un used class list")]
        public void unusedClassList()
        {

        }

        [Fact]
        [Trait("Requirement", "Find reference classes")]
        public void findReferenceClasses()
        {

        }

        [Fact]
        [Trait("Requirement", "Get service method list")]
        public void getServiceMethodList()
        {

        }

        [Fact]
        public void extractClassName()
        {

        }

        [Fact]
        [Trait("Requirement", "Extract reference variable")]
        public void extractReferenceVariable()
        {

        }

        [Fact]
        [Trait("Requirement", "Get dead code config")]
        public void GetDeadCodeConfig()
        {

        }

        [Fact]
        [Trait("Requirement", "Get state")]
        public void GetState()
        {

        }

        //[Fact]
        //[Trait("Requirement", "Service method list")]
        //public void serviceMethodList()
        //{
        //    // GetConfiguration();
        //    MethodFinder.GetExecutingDirectoryName();
        //    // MethodFinder.GetFileStream();
        //    MethodFinder.serviceMethodList();
        //    Assert.True(true);

        //}

        //[Fact]
        //[Trait("Requirement", "Get dead code config")]
        //public void GetDeadCodeConfig_ArgumentNullException()
        //{
        //    GetConfiguration();
        //    MethodFinder.outputFileName = "";
        //    MethodFinder.projectPath = "";
        //    MethodFinder.GetExecutingDirectoryName();
        //    Assert.Throws<ArgumentException>(() => MethodFinder.GetDeadCodeConfig());
        //}

        //[Fact]
        //[Trait("Requirement", "Get component List")]
        //public void componentMethodList_success()
        //{
        //    GetConfiguration();
        //    MethodFinder.GetDeadCodeConfig();
        //    MethodFinder.componentMethodList();
        //    Assert.True(true);

        //}
    }
}

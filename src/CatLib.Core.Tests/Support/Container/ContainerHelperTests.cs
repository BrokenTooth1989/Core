﻿/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CatLib.Tests
{
    [TestClass]
    public class ContainerHelperTests
    {
        /// <summary>
        /// 生成服务和转为目标
        /// </summary>
        [TestMethod]
        public void MakeTConvert()
        {
            var container = MakeContainer();
            var obj = container.Make<ContainerHelperTests>("ContainerHelperTests");
            Assert.AreSame(this, obj);
        }

        /// <summary>
        /// 生成服务和转为目标
        /// </summary>
        [TestMethod]
        public void MakeTService()
        {
            var container = MakeContainer();
            var obj = container.Make<ContainerHelperTests>();
            Assert.AreSame(this, obj);
        }

        /// <summary>
        /// 以单例形式绑定
        /// </summary>
        [TestMethod]
        public void BindSingleton()
        {
            var container = MakeContainer();
            container.Singleton("BindSingleton", (c, param) =>
            {
                return new object();
            });
            var obj = container.Make("BindSingleton");
            Assert.AreSame(obj, container.Make("BindSingleton"));
        }

        public class ContainerHelperTestClass
        {

        }

        public class TestClassService
        {

        }

        /// <summary>
        /// 以单列形式绑定
        /// </summary>
        [TestMethod]
        public void BindSingletonTServiceTConcrete()
        {
            var container = MakeContainer();
            container.Singleton<TestClassService, ContainerHelperTestClass>();
            var obj = container.Make(container.Type2Service(typeof(ContainerHelperTestClass)));
            var obj2 = container.Make(container.Type2Service(typeof(TestClassService)));

            Assert.AreSame(obj, obj2);
        }

        /// <summary>
        /// 以单列形式绑定
        /// </summary>
        [TestMethod]
        public void SingletonTService()
        {
            var container = MakeContainer();
            container.Singleton<TestClassService>((c, p) =>
            {
                return new object();
            });
            var obj = container.Make(container.Type2Service(typeof(TestClassService)));
            var obj2 = container.Make(container.Type2Service(typeof(TestClassService)));

            Assert.AreSame(obj, obj2);
        }

        [TestMethod]
        public void TestInstance()
        {
            var container = MakeContainer();
            var obj = new TestClassService();
            container.Instance<TestClassService>(obj);

            Assert.AreSame(obj, container.Make<TestClassService>());
        }

        [TestMethod]
        public void TestRelease()
        {
            var container = MakeContainer();
            var obj = new TestClassService();
            container.Instance<TestClassService>(obj);
            container.OnFindType((str) =>
            {
                return Type.GetType(str);
            });

            Assert.AreSame(obj, container.Make<TestClassService>());
            container.Release<TestClassService>();
            // 因为被释放后容器会容器会自动推测出所需类的实例
            Assert.AreSame(obj.GetType(), container.Make<TestClassService>().GetType());
        }

        [TestMethod]
        public void TestBindIf()
        {
            var app = new Application();
            IBindData bindData;
            Assert.AreEqual(true, App.BindIf("TestBind", (c, p) => 1, out bindData));
            Assert.AreEqual(false, App.BindIf("TestBind", (c, p) => 2, out bindData));
            Assert.AreEqual(1, app["TestBind"]);

            Assert.AreEqual(true, App.BindIf<object>(out bindData));
            Assert.AreEqual(typeof(object), app.Make<object>().GetType());

            Assert.AreEqual(true, App.BindIf<int>((c, p) => 100, out bindData));
            Assert.AreEqual(true, App.BindIf<double, float>(out bindData));
            Assert.AreEqual(false, App.BindIf<double, float>(out bindData));

            Assert.AreEqual(typeof(double), App.Make<double>(App.Type2Service(typeof(float))).GetType());
        }

        [TestMethod]
        public void TestSingletonIf()
        {
            var testObject = new object();
            var testObject2 = new object();
            var app = new Application();
            IBindData bindData;
            Assert.AreEqual(true, App.SingletonIf("TestBind", (c, p) => new object(), out bindData));

            var makeObject = app["TestBind"];
            Assert.AreEqual(false, App.SingletonIf("TestBind", (c, p) => testObject2, out bindData));
            Assert.AreSame(testObject.GetType(), makeObject.GetType());
            Assert.AreSame(makeObject, app["TestBind"]);

            Assert.AreEqual(true, App.SingletonIf<object>(out bindData));
            Assert.AreEqual(typeof(object), app.Make<object>().GetType());

            Assert.AreEqual(true, App.SingletonIf<int>((c, p) => 100, out bindData));
            Assert.AreEqual(true, App.SingletonIf<double, float>(out bindData));
            Assert.AreEqual(false, App.SingletonIf<double, float>(out bindData));

            Assert.AreEqual(typeof(double), App.Make<double>(App.Type2Service(typeof(float))).GetType());
        }

        /// <summary>
        /// 生成容器
        /// </summary>
        /// <returns>容器</returns>
        private Container MakeContainer()
        {
            var container = new Container();
            container.Instance("ContainerHelperTests", this);
            container.Instance(container.Type2Service(typeof(ContainerHelperTests)), this);
            return container;
        }
    }
}

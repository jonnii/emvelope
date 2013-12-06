using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Emvelope.Integration
{
    [TestFixture]
    public class DefaultsFeatures
    {
        private IDisposable application;

        [TestFixtureSetUp]
        public void SetUp()
        {
            application = WebApp.Start<Startup>("http://localhost:44543");
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            application.Dispose();
        }

        [Test]
        public async Task ShouldWrapContentsInEnvelopeByDefault()
        {
            var client = new HttpClient();
            var result = await client.GetAsync("http://localhost:44543/api/products/3");
            var contents = await result.Content.ReadAsStringAsync();

            var obj = JObject.Parse(contents);

            Assert.That(obj["product"], Is.Not.Null);
            Assert.That(obj["product"]["id"].Value<int>(), Is.EqualTo(3));
        }

        [Test]
        public async Task ShouldNotEnvelopeWhenEnvelopeQueryStringParamIsFalse()
        {
            var client = new HttpClient();
            var result = await client.GetAsync("http://localhost:44543/api/products/3?envelope=false");
            var contents = await result.Content.ReadAsStringAsync();

            var obj = JObject.Parse(contents);

            Assert.That(obj["Id"].Value<int>(), Is.EqualTo(3));
        }
    }
}
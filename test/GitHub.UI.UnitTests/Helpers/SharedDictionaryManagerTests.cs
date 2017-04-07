﻿using System;
using System.Windows;
using NUnit.Framework;

namespace GitHub.UI.Helpers.UnitTests
{
    public partial class SharedDictionaryManagerTests
    {
        public class TheSourceProperty
        {
            [TestCase(Urls.GitHub_UI_SharedDictionary_PackUrl)]
            [TestCase(Urls.GitHub_VisualStudio_UI_SharedDictionary_PackUrl)]
            [TestCase(Urls.GitHub_UI_SharedDictionary_FileUrl, Description = "This is a design time URL")]
            [TestCase(Urls.GitHub_VisualStudio_UI_SharedDictionary_FileUrl, Description = "This is a design time URL")]
            [RequiresThread(System.Threading.ApartmentState.STA)]
            public void SetSourceOnDifferentInstances_ExpectTheSameObjects(string url)
            {
                var setup = new AppDomainSetup { ApplicationBase = "NOTHING_HERE" };
                using (var context = new AppDomainContext(setup))
                {
                    var remote = context.CreateInstance<SharedDictionaryManagerContext>();
                    string expectDump = remote.DumpMergedDictionariesSharedDictionaryManager(url);

                    var dump = remote.DumpMergedDictionariesSharedDictionaryManager(url);

                    Assert.That(dump, Is.EqualTo(expectDump));
                }
            }

            // This is why we need `SharedDictionaryManager`.
            [TestCase(Urls.GitHub_UI_SharedDictionary_PackUrl)]
            [TestCase(Urls.GitHub_VisualStudio_UI_SharedDictionary_PackUrl)]
            [RequiresThread(System.Threading.ApartmentState.STA)]
            public void SetResourceDictionarySourceOnDifferentInstances_ExpectDifferentObjects(string url)
            {
                var setup = new AppDomainSetup { ApplicationBase = "NOTHING_HERE" };
                using (var context = new AppDomainContext(setup))
                {
                    var remote = context.CreateInstance<SharedDictionaryManagerContext>();
                    string expectDump = remote.DumpMergedDictionariesLoadingResourceDictionary(url);

                    var dump = remote.DumpMergedDictionariesLoadingResourceDictionary(url);

                    Assert.That(dump, Is.Not.EqualTo(expectDump));
                }
            }

            class SharedDictionaryManagerContext : MarshalByRefObject
            {
                internal string DumpMergedDictionariesLoadingResourceDictionary(string url)
                {
                    var target = new LoadingResourceDictionary();
                    return DumpMergedDictionaries(target, url);
                }

                internal string DumpMergedDictionariesSharedDictionaryManager(string url)
                {
                    var target = new SharedDictionaryManager();
                    return DumpMergedDictionaries(target, url);
                }

                string DumpMergedDictionaries(ResourceDictionary target, string url)
                {
                    SetProperty(target, "Source", ResourceDictionaryUtilities.ToPackUri(url));

                    return ResourceDictionaryUtilities.DumpResourceDictionary(target);
                }

                static void SetProperty(object target, string name, object value)
                {
                    var prop = target.GetType().GetProperty(name);
                    prop.SetValue(target, value);
                }
            }
        }
    }
}
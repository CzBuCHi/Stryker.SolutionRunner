﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Stryker.SolutionRunner {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Stryker.SolutionRunner.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {&quot;schemaVersion&quot;:&quot;1&quot;,&quot;thresholds&quot;:{&quot;high&quot;:80,&quot;low&quot;:60},&quot;projectRoot&quot;:&quot;c:\\projects\\Celebrum\\BackEnd&quot;,&quot;files&quot;:{}}.
        /// </summary>
        internal static string mutation_report {
            get {
                return ResourceManager.GetString("mutation_report", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*! For license information please see mutation-test-elements.js.LICENSE.txt */
        ///!function(){var e={8986:function(e,t,r){&quot;use strict&quot;;var n=r(9601),o=r.n(n),i=r(2609),s=r.n(i)()(o());s.push([e.id,&quot;:host{background-color:var(--bs-body-bg);font-family:-apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Helvetica Neue,Arial,Noto Sans,sans-serif,Apple Color Emoji,Segoe UI Emoji,Segoe UI Symbol,Noto Color Emoji;font-size:1rem;font-weight:400;line-height:1.15;line-height:1.5;margin:0;text-align:left}.container-fluid [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string mutation_test_elements {
            get {
                return ResourceManager.GetString("mutation_test_elements", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html&gt;
        ///    &lt;head&gt;
        ///        &lt;script&gt;##REPORT_JS##&lt;/script&gt;
        ///    &lt;/head&gt;
        ///    &lt;body&gt;
        ///        &lt;mutation-test-report-app title-postfix=&quot;##REPORT_TITLE##&quot;&gt;&lt;/mutation-test-report-app&gt;
        ///        
        ///        &lt;script&gt;
        ///            document.querySelector(&apos;mutation-test-report-app&apos;).report =
        ///                ##REPORT_JSON##;
        ///        &lt;/script&gt;
        ///    &lt;/body&gt;
        ///&lt;/html&gt;.
        /// </summary>
        internal static string ReportTemplate {
            get {
                return ResourceManager.GetString("ReportTemplate", resourceCulture);
            }
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;
using LibGit2Sharp.Core.Compat;

namespace LibGit2Sharp
{
    /// <summary>
    /// The collection of <see cref="RefSpec"/>s in a <see cref="Remote"/>
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class RefSpecCollection : IEnumerable<RefSpec>
    {
        private readonly Lazy<IList<RefSpec>> refSpecsLazy;

        /// <summary>
        /// Needed for mocking purposes.
        /// </summary>
        protected RefSpecCollection()
        { }

        internal RefSpecCollection(Remote remote)
        {
            Ensure.ArgumentNotNull(remote, "remote");

            refSpecsLazy = new Lazy<IList<RefSpec>>(() => RetrieveRefSpecs(remote));
        }

        private static IList<RefSpec> RetrieveRefSpecs(Remote remote)
        {
            using (RemoteSafeHandle remoteHandle = Proxy.git_remote_load(remote.repository.Handle, remote.Name, true))
            {
                int count = Proxy.git_remote_refspec_count(remoteHandle);
                List<RefSpec> refSpecs = new List<RefSpec>();

                for (int i = 0; i < count; i++)
                {
                    using (GitRefSpecHandle handle = Proxy.git_remote_get_refspec(remoteHandle, i))
                    {
                        refSpecs.Add(RefSpec.BuildFromPtr(handle));
                    }
                }

                return refSpecs;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<RefSpec> GetEnumerator()
        {
            return refSpecsLazy.Value.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "Count = {0}", this.Count());
            }
        }
    }
}

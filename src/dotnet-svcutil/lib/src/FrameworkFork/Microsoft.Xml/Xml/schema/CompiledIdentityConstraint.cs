// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using System.Text;
    using System.Collections;
    using System.Diagnostics;
    using Microsoft.Xml.XPath;
    using MS.Internal.Xml.XPath;

    internal class CompiledIdentityConstraint
    {
        internal XmlQualifiedName name = XmlQualifiedName.Empty;
        private ConstraintRole _role;
        private Asttree _selector;
        private Asttree[] _fields;
        internal XmlQualifiedName refer = XmlQualifiedName.Empty;

        public enum ConstraintRole
        {
            Unique,
            Key,
            Keyref
        }

        public ConstraintRole Role
        {
            get { return _role; }
        }

        public Asttree Selector
        {
            get { return _selector; }
        }

        public Asttree[] Fields
        {
            get { return _fields; }
        }

        public static readonly CompiledIdentityConstraint Empty = new CompiledIdentityConstraint();

        private CompiledIdentityConstraint() { }

        public CompiledIdentityConstraint(XmlSchemaIdentityConstraint constraint, XmlNamespaceManager nsmgr)
        {
            this.name = constraint.QualifiedName;

            //public Asttree (string xPath, bool isField, XmlNamespaceManager nsmgr)
            try
            {
                _selector = new Asttree(constraint.Selector.XPath, false, nsmgr);
            }
            catch (XmlSchemaException e)
            {
                e.SetSource(constraint.Selector);
                throw e;
            }
            XmlSchemaObjectCollection fields = constraint.Fields;
            Debug.Assert(fields.Count > 0);
            _fields = new Asttree[fields.Count];
            for (int idxField = 0; idxField < fields.Count; idxField++)
            {
                try
                {
                    _fields[idxField] = new Asttree(((XmlSchemaXPath)fields[idxField]).XPath, true, nsmgr);
                }
                catch (XmlSchemaException e)
                {
                    e.SetSource(constraint.Fields[idxField]);
                    throw e;
                }
            }
            if (constraint is XmlSchemaUnique)
            {
                _role = ConstraintRole.Unique;
            }
            else if (constraint is XmlSchemaKey)
            {
                _role = ConstraintRole.Key;
            }
            else
            {             // XmlSchemaKeyref
                _role = ConstraintRole.Keyref;
                this.refer = ((XmlSchemaKeyref)constraint).Refer;
            }
        }
    }
}



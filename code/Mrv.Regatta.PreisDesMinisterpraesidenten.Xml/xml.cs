﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:2.0.50727.8825
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Dieser Quellcode wurde automatisch generiert von xsd, Version=2.0.50727.3038.
// 
namespace Mrv.Regatta.PreisDesMinisterpraesidenten.Xml {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class Einstellungen {
        
        private EinstellungenProfil[] profilField;
        
        private string profilnameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Profil", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public EinstellungenProfil[] Profil {
            get {
                return this.profilField;
            }
            set {
                this.profilField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Profilname {
            get {
                return this.profilnameField;
            }
            set {
                this.profilnameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class EinstellungenProfil {
        
        private CT_ConnectionString connectionStringDatenDbField;
        
        private CT_ConnectionString connectionStringZeitmessDbField;
        
        private CT_Wertungsschluessel wertungsschluesselField;
        
        private string profilnameField;
        
        private string regExRennenField;
        
        private string regExLaufTypField;
        
        private EinstellungenProfilRenngemeinschaftenTeilen renngemeinschaftenTeilenField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CT_ConnectionString ConnectionStringDatenDb {
            get {
                return this.connectionStringDatenDbField;
            }
            set {
                this.connectionStringDatenDbField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CT_ConnectionString ConnectionStringZeitmessDb {
            get {
                return this.connectionStringZeitmessDbField;
            }
            set {
                this.connectionStringZeitmessDbField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CT_Wertungsschluessel Wertungsschluessel {
            get {
                return this.wertungsschluesselField;
            }
            set {
                this.wertungsschluesselField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Profilname {
            get {
                return this.profilnameField;
            }
            set {
                this.profilnameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RegExRennen {
            get {
                return this.regExRennenField;
            }
            set {
                this.regExRennenField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RegExLaufTyp {
            get {
                return this.regExLaufTypField;
            }
            set {
                this.regExLaufTypField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public EinstellungenProfilRenngemeinschaftenTeilen RenngemeinschaftenTeilen {
            get {
                return this.renngemeinschaftenTeilenField;
            }
            set {
                this.renngemeinschaftenTeilenField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CT_ConnectionString {
        
        private string providerNameField;
        
        private string connectionStringField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ProviderName {
            get {
                return this.providerNameField;
            }
            set {
                this.providerNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ConnectionString {
            get {
                return this.connectionStringField;
            }
            set {
                this.connectionStringField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CT_Punkte {
        
        private float platz1Field;
        
        private float platz2Field;
        
        private float platz3Field;
        
        private float platz4Field;
        
        private float platz5Field;
        
        private float platz6Field;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public float Platz1 {
            get {
                return this.platz1Field;
            }
            set {
                this.platz1Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public float Platz2 {
            get {
                return this.platz2Field;
            }
            set {
                this.platz2Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public float Platz3 {
            get {
                return this.platz3Field;
            }
            set {
                this.platz3Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public float Platz4 {
            get {
                return this.platz4Field;
            }
            set {
                this.platz4Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public float Platz5 {
            get {
                return this.platz5Field;
            }
            set {
                this.platz5Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public float Platz6 {
            get {
                return this.platz6Field;
            }
            set {
                this.platz6Field = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CT_Wertungsschluessel {
        
        private CT_Punkte einerField;
        
        private CT_Punkte zweierField;
        
        private CT_Punkte viererField;
        
        private CT_Punkte achterField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CT_Punkte Einer {
            get {
                return this.einerField;
            }
            set {
                this.einerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CT_Punkte Zweier {
            get {
                return this.zweierField;
            }
            set {
                this.zweierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CT_Punkte Vierer {
            get {
                return this.viererField;
            }
            set {
                this.viererField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CT_Punkte Achter {
            get {
                return this.achterField;
            }
            set {
                this.achterField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public enum EinstellungenProfilRenngemeinschaftenTeilen {
        
        /// <remarks/>
        NichtTeilen,
        
        /// <remarks/>
        TeilenOhnePunkte,
        
        /// <remarks/>
        TeilenMitPunkte,
    }
}

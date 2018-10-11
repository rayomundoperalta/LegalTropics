namespace LegalTropics
{
    partial class Tropicalizador : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Tropicalizador()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.Analyzer = this.Factory.CreateRibbonGroup();
            this.ipaddress = this.Factory.CreateRibbonEditBox();
            this.buttonAnalyzer = this.Factory.CreateRibbonButton();
            this.StartLocalServer = this.Factory.CreateRibbonButton();
            this.Párrafos = this.Factory.CreateRibbonGroup();
            this.buttonParrafos = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.Analyzer.SuspendLayout();
            this.Párrafos.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.Analyzer);
            this.tab1.Groups.Add(this.Párrafos);
            this.tab1.Label = "Tropicalizador";
            this.tab1.Name = "tab1";
            // 
            // Analyzer
            // 
            this.Analyzer.Items.Add(this.ipaddress);
            this.Analyzer.Items.Add(this.buttonAnalyzer);
            this.Analyzer.Items.Add(this.StartLocalServer);
            this.Analyzer.Label = "Tropicalizador";
            this.Analyzer.Name = "Analyzer";
            // 
            // ipaddress
            // 
            this.ipaddress.Label = "ip Address";
            this.ipaddress.Name = "ipaddress";
            this.ipaddress.Text = "10.3.2.135";
            this.ipaddress.TextChanged += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ipaddress_TextChanged);
            // 
            // buttonAnalyzer
            // 
            this.buttonAnalyzer.Label = "Analyzer";
            this.buttonAnalyzer.Name = "buttonAnalyzer";
            this.buttonAnalyzer.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonAnalyzer_Click);
            // 
            // StartLocalServer
            // 
            this.StartLocalServer.Label = "Start Local Server";
            this.StartLocalServer.Name = "StartLocalServer";
            this.StartLocalServer.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.StartLocalServer_Click);
            // 
            // Párrafos
            // 
            this.Párrafos.Items.Add(this.buttonParrafos);
            this.Párrafos.Label = "Párrafos";
            this.Párrafos.Name = "Párrafos";
            // 
            // buttonParrafos
            // 
            this.buttonParrafos.Label = "Parrafos";
            this.buttonParrafos.Name = "buttonParrafos";
            this.buttonParrafos.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonParrafos_Click);
            // 
            // Tropicalizador
            // 
            this.Name = "Tropicalizador";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Tropicalizador_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.Analyzer.ResumeLayout(false);
            this.Analyzer.PerformLayout();
            this.Párrafos.ResumeLayout(false);
            this.Párrafos.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup Analyzer;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonAnalyzer;
        internal Microsoft.Office.Tools.Ribbon.RibbonEditBox ipaddress;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton StartLocalServer;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup Párrafos;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonParrafos;
    }

    partial class ThisRibbonCollection
    {
        internal Tropicalizador Tropicalizador
        {
            get { return this.GetRibbon<Tropicalizador>(); }
        }
    }
}

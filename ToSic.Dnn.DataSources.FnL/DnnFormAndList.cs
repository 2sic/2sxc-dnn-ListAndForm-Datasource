using System.Collections.Generic;
using DotNetNuke.Modules.UserDefinedTable;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.VisualQuery;

namespace ToSic.Dnn.DataSources
{
    /// <inheritdoc />
    /// <summary>
    /// Delivers UDT-data (now known as Form and List) to the templating engine
    /// </summary>
    [VisualQuery(
        GlobalName = "0a0924a5-ca2f-4db5-8fc7-1a21fdbb2fbb",
        NiceName = "Dnn FormAndList",
        PreviousNames = new []{
            "Environment.Dnn7.DataSources.DnnFormAndList, ToSic.SexyContent",
            "ToSic.SexyContent.DataSources.DnnFormAndList, ToSic.SexyContent" },
        Type = DataSourceType.Source, 
        ExpectsDataOfType = "d98db323-7c33-4f2a-b173-ef91c0875124",
        HelpLink = "https://github.com/2sic/dnn-datasource-form-and-list/wiki")] 

    public sealed class DnnFormAndList : ExternalDataDataSource
    {

        public override string LogId => "Dnn.Ds-FnL";                   // this text is added to all internal logs, so it's easier to debug

        const string ModuleIdConfigKey = "ModuleId";            // key in the configuration list for module Id
        const string TitleFieldConfigKey = "TitleField";        // key in the configuration list for the title filed
        const string ContentTypeConfigKey = "ContentType";      // key in the configuration list for the content-type name
        const string ModIdSetName = "ModuleId";
        const string TitleFieldSetName = "TitleFieldName";
        const string ContentTypeSetName = "ContentTypeName";

        /// <summary>
        /// Get the FnL ModuleID (which contains the data) from the configuration
        /// </summary>
        private int ModuleId => int.Parse(Configuration[ModuleIdConfigKey]);

        /// <summary>
        /// Name of the Title Attribute of the Source DataTable from the configuration
        /// </summary>
        private string TitleField => Configuration[TitleFieldConfigKey];

        /// <summary>
        /// Name of the ContentType Attribute from the configuration
        /// </summary>
        private string ContentType => Configuration[ContentTypeConfigKey];


        /// <summary>
        /// Initializes this data source
        /// </summary>
        public DnnFormAndList()
        {
            // Specify what out-streams this data-source provides. Usually just one, called "Default"
            Provide(GetList);

            // Register the configurations we want as tokens, so that the values will be injected later on
            ConfigMask(ModuleIdConfigKey, $"[Settings:{ModIdSetName}||0]");
            ConfigMask(TitleFieldConfigKey, $"[Settings:{TitleFieldSetName}]");
            ConfigMask(ContentTypeConfigKey, $"[Settings:{ContentTypeSetName}||FnL]");
        }

        /// <summary>
        /// Internal helper that returns the entities - actually just retrieving them from the attached Data-Source
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Eav.Interfaces.IEntity> GetList() => LoadFnL().List;


        private DataTableDataSource LoadFnL()
        {
            // check if already loaded, if yes, return that
            if (_dtDs != null) return _dtDs;

            // this parses the config-tokens, resulting in the values we will use in the code below
            EnsureConfigurationIsLoaded();

            // Preferred way in Form and List - use GetDataSet of ModuleId
            var udt = new UserDefinedTableController();
            var ds = udt.GetDataSet(ModuleId);

            // now build a DataTableDataSource to pass on
            _dtDs = DataSource.GetDataSource<DataTableDataSource>(valueCollectionProvider: ConfigurationProvider, parentLog: Log);
            _dtDs.Source = ds.Tables["Data"];           // the data-table of FnL/UDT
            _dtDs.EntityIdField = "UserDefinedRowId";   // default column created by FnL/UDT
            _dtDs.ContentType = ContentType;            // a type name what these items are called afterwards

            // clean up column names if possible, remove spaces in the column-names as this would cause trouble in Razor templates
            for (var i = 0; i < _dtDs.Source.Columns.Count; i++)
                _dtDs.Source.Columns[i].ColumnName = _dtDs.Source.Columns[i].ColumnName
                    .Replace(" ", "");

            // Set the title-field - either the configured one, or if missing, just the first column we find
            _dtDs.TitleField = string.IsNullOrWhiteSpace(TitleField) 
                ? _dtDs.Source.Columns[1].ColumnName 
                : TitleField;
            return _dtDs;
        }
        private DataTableDataSource _dtDs;



    }
}

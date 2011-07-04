using Raven.Abstractions.Data;
using Raven.Studio.Infrastructure.Navigation;

namespace Raven.Studio.Features.Documents
{
	using System.ComponentModel.Composition;
	using System.Linq;
	using System.Threading.Tasks;
	using Caliburn.Micro;
	using Framework;
	using Messages;
	using Plugins;
	using Plugins.Database;

    [Export]
	[ExportDatabaseExplorerItem("Documents", Index = 40)]
	public class BrowseDocumentsViewModel : RavenScreen,
		IHandle<DocumentDeleted>
	{
		string status;

		[ImportingConstructor]
		public BrowseDocumentsViewModel()
		{
			DisplayName = "Documents";
			Events.Subscribe(this);

			Server.CurrentDatabaseChanged += delegate
			{
				Initialize();
			};
		}

		void Initialize()
		{
			Status = "Retrieving documents.";

			Documents = new BindablePagedQuery<JsonDocument, DocumentViewModel>(
				GetDocumentsQuery,
				jdoc => new DocumentViewModel(jdoc));

			Documents.PageSize = 25;

			NotifyOfPropertyChange("");
		}

		protected override void OnInitialize()
		{
			Initialize();
		}

		public string Status
		{
			get { return status; }
			set { status = value; NotifyOfPropertyChange(() => Status); }
		}

		Task<JsonDocument[]> GetDocumentsQuery(int start, int pageSize)
		{
			using (var session = Server.OpenSession())
				return session.Advanced.AsyncDatabaseCommands
					.GetDocumentsAsync(start, pageSize);
		}

		public BindablePagedQuery<JsonDocument, DocumentViewModel> Documents { get; private set; }

		void IHandle<DocumentDeleted>.Handle(DocumentDeleted message)
		{
			if (Documents == null) return;

			var deleted = Documents.Where(x => x.Id == message.DocumentId).FirstOrDefault();

			if (deleted != null)
				Documents.Remove(deleted);
		}

		public void CreateNewDocument()
		{
			var doc = IoC.Get<EditDocumentViewModel>();
			Events.Publish(new DatabaseScreenRequested(() => doc));
		}

		public bool HasDocuments { get { return Documents.Any(); } }

		protected override void OnActivate()
		{
			if (Documents == null) return;

			var countOfDocuments = Server.Statistics.CountOfDocuments;

			Status = countOfDocuments == 0
				? "The database contains no documents."
				: string.Empty;

			if (countOfDocuments > 0)
				RefreshDocuments(countOfDocuments);
		}

		public void RefreshDocuments(long total)
		{
			WorkStarted("retrieving documents");
			Documents.GetTotalResults = () => total;
			Documents.LoadPage(() =>
			{
				NotifyOfPropertyChange(() => HasDocuments);
				WorkCompleted("retrieving documents");
			});
		}
	}
}
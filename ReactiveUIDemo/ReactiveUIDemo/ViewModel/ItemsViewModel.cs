using System;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUIDemo.Model;

namespace ReactiveUIDemo.ViewModel
{
    public class ItemsViewModel : ViewModelBase
    {
        /// <summary>
        /// Reactive List https://reactiveui.net/docs/handbook/collections/reactive-list
        /// </summary>
        ReactiveList<Todo> _todos;
        public ReactiveList<Todo> Todos
        {
            get => _todos;
            set => this.RaiseAndSetIfChanged(ref _todos, value);
        }
        private Todo _selectedTodo;
        public Todo SelectedTodo
        {
            get => _selectedTodo;
            set => this.RaiseAndSetIfChanged(ref _selectedTodo, value);
        }

        private string _todoTitl;
        public string TodoTitle
        {
            get { return _todoTitl; }
            set { this.RaiseAndSetIfChanged(ref _todoTitl, value); }
        }

        public ReactiveCommand AddCommand { get; private set; }

        public ItemsViewModel(IScreen hostScreen = null) : base(hostScreen)
        {
            var canAdd = this.WhenAnyValue(x => x.TodoTitle, title => !String.IsNullOrEmpty(title));

            AddCommand = ReactiveCommand.Create(() =>
            {
                Todos.Add(new Todo() { Title = TodoTitle });
                TodoTitle = string.Empty;
            }, canAdd);

            //Dont forget to set ChangeTrackingEnabled to true.
            Todos = new ReactiveList<Todo>() { ChangeTrackingEnabled = true };

            Todos.Add(new Todo { IsDone = false, Title = "Go to Sleep" });
            Todos.Add(new Todo { IsDone = false, Title = "Go get some dinner" });
            Todos.Add(new Todo { IsDone = false, Title = "Watch GOT" });
            Todos.Add(new Todo { IsDone = false, Title = "Code code and code!!!!" });

            ///Lets detect when ever a todo Item is marked as done 
            ///IF it is, it is sent to the bottom of the list
            ///Else nothing happens
            Todos
                .ItemChanged
                .Where(x => x.PropertyName == "IsDone" && x.Sender.IsDone)
                .Select(x => x.Sender)
                .Subscribe(x =>
                {
                    if (x.IsDone)
                    {
                        Todos.Remove(x);
                        Todos.Add(x);
                    }
                });
        }
    }
}

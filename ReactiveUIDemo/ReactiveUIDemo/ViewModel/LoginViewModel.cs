using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using ReactiveUI;
using ReactiveUIDemo.Services;

namespace ReactiveUIDemo.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        ILogin _loginService;

        private string _userName;
        public string UserName
        {
            get => _userName;
            //Notify when property user name changes
            set => this.RaiseAndSetIfChanged(ref _userName, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public ReactiveCommand<Unit, bool> LoginCommand { get; private set; }

        public LoginViewModel(ILogin login, IScreen hostScreen = null) : base(hostScreen)
        {
            _loginService = login;

            var canLogin = this.WhenAnyValue(x => x.UserName, x => x.Password,
                (email, password) =>
                (
                    //Validate the password
                    !string.IsNullOrEmpty(password) && password.Length > 5
                )
                &&
                (
                    //Validate the email.
                    !string.IsNullOrEmpty(email)
                    &&
                    Regex.Matches(email, "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$").Count == 1
                ));

            LoginCommand = ReactiveCommand.CreateFromTask(_ => login.Login(_userName, _password), canLogin);

            LoginCommand
                .Where(logged => logged)
                .SelectMany(logged => HostScreen
                    .Router
                    .Navigate
                    .Execute(new ItemsViewModel()))
                .Subscribe();
        }
    }
}

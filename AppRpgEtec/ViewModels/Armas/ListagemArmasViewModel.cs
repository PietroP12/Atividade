using AppRpgEtec.Models;
using AppRpgEtec.Services.Armas;
using AppRpgEtec.Services.Personagens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;


namespace AppRpgEtec.ViewModels.Armas
{

    public class ListagemArmasViewModel : BaseViewModel
    {
        private ArmaService pService;
        public ObservableCollection<Arma> Armas { get; set; }

        public ListagemArmasViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new ArmaService(token);
            Armas = new ObservableCollection<Arma>();

            _ = ObterArmas();

            NovoArmaCommand = new Command(async () => { await ExibirCadastroArma(); });
            RemoverArmaCommand = new Command<Arma>(async (Arma p) => { await RemoverArma(p); });
        }

        public ICommand NovoArmaCommand { get; }
        public ICommand RemoverArmaCommand { get; set; }


        public async Task ObterArmas()
        {
            try
            {
                Armas = await pService.GetArmasAsync();
                OnPropertyChanged(nameof(Armas));
            }
            catch (Exception ex)
            {
                {
                    await Application.Current.MainPage.DisplayAlertAsync("Ops", ex.Message + "Detalhes:" + ex.InnerException, "Ok");
                }
            }
        }

        public async Task ExibirCadastroArma()
        {
            try
            {
                await Shell.Current.GoToAsync("cadArmaView");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlertAsync("Ops", ex.Message + "Detalhes" + ex.InnerException, "Ok");



            }
        }
        private Arma armaSelecionado;
        public Arma ArmaSelecionado
        {
            get { return armaSelecionado; }
            set
            {
                if (value != null)
                {
                    armaSelecionado = value;

                    Shell.Current
                        .GoToAsync($"cadArmaView?pId={armaSelecionado.Id}");
                }
            }
        }
        public async Task RemoverArma(Arma p)
        {
            try
            {
                if (await Application.Current.MainPage
                    .DisplayAlert("Confirmação", $"Confirma a remoção de {p.Nome}?", "Sim", "Não"))
                {
                    await pService.DeleteArmaAsync(p.Id);

                    await Application.Current.MainPage.DisplayAlertAsync("Mensagem", "Personagem removido com sucesso!", "Ok");

                    _ = ObterArmas();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlertAsync("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }

    }
    }

﻿using Huggy.Client.Api.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Huggy.Client.Api.Services
{
    public class ChatServiceApi
    {

        private Uri baseAddress = new Uri("https://api.huggy.io/v2");
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Obter os Chats da api do HUGGY
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string> ListAllChatsJson(string token, string uri = "https://api.huggy.io/v2")
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var stringTask = await client.GetStringAsync($"{uri}/chats");

                return stringTask;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Listar todos os chats que encontrar no serviço do Huggy
        /// </summary>
        /// <param name="token">token para autenticação na api</param>
        /// <param name="page">pagina a qual se deseja começar a busca, padrão = 0</param>
        /// <param name="allPages">se falso retorna apenas a pagina solicitada</param>
        /// <returns>Uma lista do tipo <seealso cref="List{Chat}"/>  </returns>
        public async Task<List<Chat>> ListAllChats(string token, int page = 0, bool allPages = true, string uri = "https://api.huggy.io/v2")
        {
            if (token.Length < 30) throw new ArgumentException("O parametro token é invalido!");

            int result;
            List<Chat> list = new List<Chat>();

            try
            {
                do
                {
                    result = 0;

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var serializer = new DataContractJsonSerializer(typeof(List<Chat>));
                    var streamTask = await client.GetStreamAsync($"{uri}/chats?page={page}");

                    var myList = serializer.ReadObject(streamTask) as List<Chat>;
                    list.AddRange(myList);

                    result = myList.Count;
                    page++;

                } while (result >= 20 && allPages);

                return list;
            }
            catch (AggregateException ag)
            {
                Console.WriteLine($"Erro de agregação no objeto " + ag.Message);
                throw ag;
            }
            catch (ArgumentNullException a)
            {
                Console.WriteLine("Falta um argumento para processar a solicitação. " + a.Message);
                throw a;
            }
            catch (Exception e)
            {
                Console.WriteLine("Um erro ocorreu! " + e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Busca um chat pelo seu identificador unico
        /// </summary>
        /// <param name="token">token para autenticação na api</param>
        /// <param name="id">idetnificador unico do chat no sistema</param>
        /// <returns>retorna um objeto do tipo <see cref="Chat"/></returns>
        public async Task<Chat> GetChat(string token, long id, string uri = "https://api.huggy.io/v2")
        {
            if (token.Length < 30 || id == 0) throw new ArgumentException("O parametro é invalido!");

            try
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var serializer = new DataContractJsonSerializer(typeof(Chat));
                var streamTask = await client.GetStreamAsync($"{uri=-}/chats/{id}");

                var chat = serializer.ReadObject(streamTask) as Chat;

                return chat;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPO_Cursovaya.Models;

namespace KPO_Cursovaya.Implements
{
    public class RequestStorage
    {
        public void Insert(Request request)
        {
            using var db = new DiplomContext();
            db.Requests.Add(request);
            db.SaveChanges();
        }
        public void Update(Request request)
        {
            using var db = new DiplomContext();
            var element = db.Requests.FirstOrDefault(rec => rec.Id == request.Id);
            if (element == null)
                throw new Exception("Не найдено");
            CreateModel(request, element);
            db.SaveChanges();
        }
        private Request CreateModel(Request model, Request request)
        {
            request.Date = model.Date;
            request.Text = model.Text;
            request.Url = model.Url;
            request.Author = model.Author;
            request.AuthorId = model.AuthorId;
            request.AuthorLink = model.AuthorLink;
            return request;

        }
        public List<Request> GetFullList()
        {
            using var db = new DiplomContext();
            return db.Requests.ToList();
        }
        public Request GetById(string id)
        {
            using var db = new DiplomContext();
            return db.Requests.Find(id);
        }
        public void Delete(string id)
        {
            using var db = new DiplomContext();
            db.Requests.Remove(db.Requests.Find(id));
            db.SaveChanges();
        }
        public void Delete(Request request)
        {
            using var db = new DiplomContext();
            db.Requests.Remove(request);
            db.SaveChanges();
        }
    }
}

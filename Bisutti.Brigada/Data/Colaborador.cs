﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Bisutti.Brigada.Data
{
	public class Colaborador : DataAccessBase<Model.Colaborador>
	{
		public override void Update(Model.Colaborador entity)
		{
			Model.Colaborador original = context.Colaboradores.FirstOrDefault(l => l.Id == entity.Id);
			context.Entry(original).CurrentValues.SetValues(entity);
			context.Entry(original).State = System.Data.Entity.EntityState.Modified;
			context.SaveChanges();
		}
		public override System.Data.Entity.Infrastructure.DbEntityEntry GetCurrent(Model.Colaborador entity)
		{
			return context.Entry(entity);
		}
		public override void Insert(Model.Colaborador entity)
		{
			context.Colaboradores.Add(entity);
			context.SaveChanges();
		}
		protected override List<Model.Colaborador> GetCollection()
		{
			return context.Colaboradores.OrderBy(c => c.Nome).ToList();
		}
		public List<Model.Colaborador> Filter(string nome, string email, string telefone)
		{
			return context.Colaboradores.OrderBy(c => c.Nome).Where(c =>
				(c.Nome.ToLower().IndexOf(nome.ToLower()) == 0 || nome.Length <= 3) &&
				(c.Email.ToLower().IndexOf(email.ToLower()) == 0 || email.Length <= 3) &&
				(c.Telefone.ToLower().IndexOf(telefone.ToLower()) == 0 || telefone.Length <= 3)
				).ToList();
		}
		public List<Model.Colaborador> GetBrigada(DateTime inicio, DateTime termino)
		{
			List<Model.Colaborador> colaboradores = context.Colaboradores
				.Include(c => c.Eventos)
				.Include(c => c.Eventos.Select(e => e.TipoBrigada))
				.Include(c => c.Eventos.Select(e => e.Evento))
				.Include(c => c.Eventos.Select(e => e.Evento.Produtora))
				.Include(c => c.Eventos.Select(e => e.Evento.Localizacao))
				.Where(c => c.Eventos.Where(b => b.Evento.Data.CompareTo(inicio) >= 0 && b.Evento.Data.CompareTo(termino) <= 0).Count() > 0)
				.OrderBy(c => c.Nome)
				.ToList();
			foreach (Model.Colaborador c in colaboradores)
				c.Eventos = c.Eventos.Where(e => e.Evento.Data.CompareTo(inicio) >= 0 && e.Evento.Data.CompareTo(termino) <= 0).OrderBy(e => e.Evento.Data).ToList();
			return colaboradores;
		}
		public List<Model.Colaborador> Filter(string nome, bool dispCerimonial, bool dispChapelaria, bool dispRecepcao, bool dispProducao, object disponibilidadeDiaria)
		{
			//return context.Colaboradores.ToList();
			int idDisponibilidadeDiaria = 0;
			if (disponibilidadeDiaria != null)
				idDisponibilidadeDiaria = (int)disponibilidadeDiaria;
			List<Model.Colaborador> colaboradores = new List<Model.Colaborador>();
			foreach (Model.Colaborador c in context.Colaboradores)
				if (
					((c.Nome != string.Empty && nome != string.Empty && c.Nome.ToLower().Replace(nome.ToLower(), "") != c.Nome.ToLower()) || nome == string.Empty)
					&& (c.DisponivelCerimonial || !dispCerimonial) && (c.DisponivelChapelaria || !dispChapelaria) && (c.DisponivelRecepcao || !dispRecepcao) && (c.DisponivelProducao || !dispProducao)
					&& ((c.DisponibilidadeId == idDisponibilidadeDiaria) || idDisponibilidadeDiaria == 0)
					)
					colaboradores.Add(c);
			return colaboradores;
		}
		public void InsertRange(List<Model.Colaborador> colaboradores)
		{
			context.Colaboradores.AddRange(colaboradores);
			context.SaveChanges();
		}
	}
}
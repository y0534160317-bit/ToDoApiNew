using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class Service
    {
        private readonly ToDoDbContext _db;

        public Service(ToDoDbContext db)
        {
            _db = db;
        }

        public async Task<List<Item>> GetAllAsync()
        {
            return await _db.Items.ToListAsync();
        }

        public async Task<Item?> GetItemAsync(int id)
        {
            return await _db.Items.FindAsync(id);
        }

        public async Task<Item> AddItemAsync(Item newItem)
        {
            _db.Items.Add(newItem);
            await _db.SaveChangesAsync();
            return newItem;
        }

        public async Task<Item?> UpdateItemAsync(int id, Item updated)
        {
            var existing = await _db.Items.FindAsync(id);
            if (existing == null) return null;
            existing.Name = updated.Name;
            existing.IsComplete = updated.IsComplete;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var existing = await _db.Items.FindAsync(id);
            if (existing == null) return false;
            _db.Items.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

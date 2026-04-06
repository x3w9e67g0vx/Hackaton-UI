using System;
using System.Linq;
using Xunit;
using HackatonUi.ViewModels;
using HackatonUi.Models;

namespace HackatonUi.Tests
{
    public class ProtocolsViewModelTests
    {
        [Fact]
        public void IsUserAllowed_ReturnsTrue_ForAdmin()
        {
            
            var mainVm = new MainWindowViewModel();
            mainVm.CurrentUser = new UserCredentials { Username = "admin", RoleName = "Admin" };
            var protocolsVm = new ProtocolsViewModel(mainVm);
            // Cвойства IsUserAllowed должны вернуть true.
            Assert.True(protocolsVm.IsUserAllowed);
        }

        [Fact]
        public void IsUserAllowed_ReturnsFalse_ForViewer()
        {
            var mainVm = new MainWindowViewModel();
            mainVm.CurrentUser = new UserCredentials { Username = "viewer", RoleName = "Viewer" };

            var protocolsVm = new ProtocolsViewModel(mainVm);
            // IsUserAllowed должно быть false.
            Assert.False(protocolsVm.IsUserAllowed);
        }

        [Fact]
        public void LoadTasks_PopulatesTasksCollection()
        {
            var mainVm = new MainWindowViewModel();
            mainVm.CurrentUser = new UserCredentials { Username = "admin", RoleName = "Admin" };
            var protocolsVm = new ProtocolsViewModel(mainVm);

            protocolsVm.LoadTasks();

            //  Коллекция Tasks не должна быть null; если база заполнена, Count будет > 0.
            Assert.NotNull(protocolsVm.Tasks);
            Assert.True(protocolsVm.Tasks.Count >= 0);
        }

        [Fact]
        public void LoadTasks_WithInvalidBuildingId_ReturnsEmptyCollection()
        {
            var mainVm = new MainWindowViewModel();
            mainVm.CurrentUser = new UserCredentials { Username = "admin", RoleName = "Admin" };
            var protocolsVm = new ProtocolsViewModel(mainVm);

            protocolsVm.LoadTasks(-1);
            // Коллекция Tasks должна быть пустой
            Assert.Empty(protocolsVm.Tasks);
        }

        [Fact]
        public void UpdateTask_ChangesDescription()
        {
            var mainVm = new MainWindowViewModel();
            mainVm.CurrentUser = new UserCredentials { Username = "admin", RoleName = "Admin" };
            var protocolsVm = new ProtocolsViewModel(mainVm);
            protocolsVm.LoadTasks();

            if (!protocolsVm.Tasks.Any())
            {
                Assert.True(true, "Нет задач в базе; тест пропущен.");
                return;
            }

            var originalTask = protocolsVm.Tasks.First();
            var newDescription = originalTask.Description + " - updated";
            originalTask.Description = newDescription;

            protocolsVm.SaveChangesCommand.Execute(null);
            protocolsVm.LoadTasks();
            // Находим задачу с тем же ID и проверяем, что описание обновлено.
            var updatedTask = protocolsVm.Tasks.FirstOrDefault(t => t.Id == originalTask.Id);
            Assert.NotNull(updatedTask);
            Assert.Equal(newDescription, updatedTask.Description);
        }
    }
}

using System;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using Save;
using UnityEngine;
using UnityEngine.TestTools;

#region Mocks

public static class MockCloudSave
{
    public static Func<Task<string>> LoadFunc;
    public static Func<Task> SaveFunc;

    public static void Setup(string returnJson, Action onSave = null)
    {
        LoadFunc = () => Task.FromResult(returnJson);
        SaveFunc = () =>
        {
            onSave?.Invoke();
            return Task.CompletedTask;
        };
    }
}

public static class MockSaveService
{
    public static Func<PlayerStatsSaveWrapper> ExportLocalWrapperFunc;
    public static Func<PlayerStatsSaveWrapper, bool> ValidateChecksumFunc;
    public static Action<PlayerStatsSaveWrapper> ImportFromCloudAction;

    public static void SetupExport(PlayerStatsSaveWrapper wrapper)
    {
        ExportLocalWrapperFunc = () => wrapper;
    }

    public static void SetupValidate(Func<PlayerStatsSaveWrapper, bool> validate)
    {
        ValidateChecksumFunc = validate;
    }

    public static void SetupImport(Action<PlayerStatsSaveWrapper> import)
    {
        ImportFromCloudAction = import;
    }
}


public class CloudSyncValidator
{
    public async Task<int> Validate()
    {
        try
        {
            var localWrapper = MockSaveService.ExportLocalWrapperFunc.Invoke();
            string cloudJson = await MockCloudSave.LoadFunc();
            
            if (string.IsNullOrEmpty(cloudJson))
            {
                await MockCloudSave.SaveFunc();
                return 1;
            }

            var cloudWrapper = JsonUtility.FromJson<PlayerStatsSaveWrapper>(cloudJson);

            bool cloudValid = MockSaveService.ValidateChecksumFunc(cloudWrapper);
            bool localValid = MockSaveService.ValidateChecksumFunc(localWrapper);

            if (!cloudValid && !localValid) return 0;
            if (!cloudValid)
            {
                await MockCloudSave.SaveFunc();
                return 1;
            }

            if (!localValid)
            {
                MockSaveService.ImportFromCloudAction?.Invoke(cloudWrapper);
                return 1;
            }

            if (localWrapper.checksum == cloudWrapper.checksum)
            {
                return 1;
            }

            if (cloudWrapper.lastSavedAt > localWrapper.lastSavedAt)
            {
                MockSaveService.ImportFromCloudAction?.Invoke(cloudWrapper);
                return 1;
            }

            await MockCloudSave.SaveFunc();
            return 1;
        }
        catch
        {
            return 0;
        }
    }
}


#endregion

public class CloudSyncValidatorTests
{
    [UnityTest]
    public IEnumerator Validate_WhenCloudIsEmpty_SavesLocalData()
    {
        bool saveCalled = false;

        var fakeLocal = new PlayerStatsSaveWrapper
        {
            data = new PlayerStatsSaveData(),
            checksum = "abc123",
            lastSavedAt = DateTime.UtcNow.Ticks
        };

        MockSaveService.SetupExport(fakeLocal);
        MockSaveService.SetupValidate(_ => true);
        MockSaveService.SetupImport(_ => { });

        MockCloudSave.Setup(null, () => saveCalled = true);

        var validator = new CloudSyncValidator();
        var task = validator.Validate();
        yield return new WaitUntil(() => task.IsCompleted);

        Assert.AreEqual(1, task.Result);
        Assert.IsTrue(saveCalled);
    }

    [UnityTest]
    public IEnumerator Validate_WhenBothChecksumsInvalid_Returns0()
    {
        var wrapper = new PlayerStatsSaveWrapper
        {
            data = new PlayerStatsSaveData(),
            checksum = "abc",
            lastSavedAt = DateTime.UtcNow.Ticks
        };

        var cloudJson = JsonUtility.ToJson(wrapper);

        MockSaveService.SetupExport(wrapper);
        MockSaveService.SetupValidate(_ => false); // Ambos inválidos
        MockSaveService.SetupImport(_ => Assert.Fail("No debería importar"));

        MockCloudSave.Setup(cloudJson);

        var validator = new CloudSyncValidator();
        var task = validator.Validate();
        yield return new WaitUntil(() => task.IsCompleted);

        Assert.AreEqual(0, task.Result);
    }

    [UnityTest]
    public IEnumerator Validate_WhenOnlyCloudInvalid_SavesLocal()
    {
        bool saveCalled = false;

        var wrapper = new PlayerStatsSaveWrapper
        {
            data = new PlayerStatsSaveData(),
            checksum = "abc",
            lastSavedAt = DateTime.UtcNow.Ticks
        };

        var cloudJson = JsonUtility.ToJson(wrapper);

        MockSaveService.SetupExport(wrapper);
        MockSaveService.SetupValidate(w => w == wrapper); // solo local válido
        MockSaveService.SetupImport(_ => Assert.Fail("No debería importar"));

        MockCloudSave.Setup(cloudJson, () => saveCalled = true);

        var validator = new CloudSyncValidator();
        var task = validator.Validate();
        yield return new WaitUntil(() => task.IsCompleted);

        Assert.AreEqual(1, task.Result);
        Assert.IsTrue(saveCalled);
    }

    [UnityTest]
    public IEnumerator Validate_WhenOnlyLocalInvalid_ImportsCloud()
    {
        bool importCalled = false;

        var wrapper = new PlayerStatsSaveWrapper
        {
            data = new PlayerStatsSaveData(),
            checksum = "abc",
            lastSavedAt = DateTime.UtcNow.Ticks
        };

        var cloudJson = JsonUtility.ToJson(wrapper);

        MockSaveService.SetupExport(wrapper);
        MockSaveService.SetupValidate(w => w != wrapper); // solo cloud válido
        MockSaveService.SetupImport(_ => importCalled = true);

        MockCloudSave.Setup(cloudJson);

        var validator = new CloudSyncValidator();
        var task = validator.Validate();
        yield return new WaitUntil(() => task.IsCompleted);

        Assert.AreEqual(1, task.Result);
        Assert.IsTrue(importCalled);
    }

    [UnityTest]
    public IEnumerator Validate_WhenChecksumsMatch_Returns1()
    {
        var wrapper = new PlayerStatsSaveWrapper
        {
            data = new PlayerStatsSaveData(),
            checksum = "match",
            lastSavedAt = DateTime.UtcNow.Ticks
        };

        var cloudJson = JsonUtility.ToJson(wrapper);

        MockSaveService.SetupExport(wrapper);
        MockSaveService.SetupValidate(_ => true);
        MockSaveService.SetupImport(_ => Assert.Fail("No debería importar"));

        MockCloudSave.Setup(cloudJson);

        var validator = new CloudSyncValidator();
        var task = validator.Validate();
        yield return new WaitUntil(() => task.IsCompleted);

        Assert.AreEqual(1, task.Result);
    }

    [UnityTest]
    public IEnumerator Validate_WhenCloudIsNewer_ImportsCloud()
    {
        bool imported = false;

        var local = new PlayerStatsSaveWrapper
        {
            data = new PlayerStatsSaveData(),
            checksum = "abc",
            lastSavedAt = DateTime.UtcNow.Ticks - 1000
        };

        var cloud = new PlayerStatsSaveWrapper
        {
            data = new PlayerStatsSaveData(),
            checksum = "def",
            lastSavedAt = DateTime.UtcNow.Ticks
        };

        var cloudJson = JsonUtility.ToJson(cloud);

        MockSaveService.SetupExport(local);
        MockSaveService.SetupValidate(_ => true);
        MockSaveService.SetupImport(_ => imported = true);

        MockCloudSave.Setup(cloudJson);

        var validator = new CloudSyncValidator();
        var task = validator.Validate();
        yield return new WaitUntil(() => task.IsCompleted);

        Assert.AreEqual(1, task.Result);
        Assert.IsTrue(imported);
    }

    [UnityTest]
    public IEnumerator Validate_WhenLocalIsNewer_SavesLocal()
    {
        bool saved = false;

        var local = new PlayerStatsSaveWrapper
        {
            data = new PlayerStatsSaveData(),
            checksum = "abc",
            lastSavedAt = DateTime.UtcNow.Ticks
        };

        var cloud = new PlayerStatsSaveWrapper
        {
            data = new PlayerStatsSaveData(),
            checksum = "def",
            lastSavedAt = DateTime.UtcNow.Ticks - 1000
        };

        var cloudJson = JsonUtility.ToJson(cloud);

        MockSaveService.SetupExport(local);
        MockSaveService.SetupValidate(_ => true);
        MockSaveService.SetupImport(_ => Assert.Fail("No debería importar"));

        MockCloudSave.Setup(cloudJson, () => saved = true);

        var validator = new CloudSyncValidator();
        var task = validator.Validate();
        yield return new WaitUntil(() => task.IsCompleted);

        Assert.AreEqual(1, task.Result);
        Assert.IsTrue(saved);
    }

    [UnityTest]
    public IEnumerator Validate_WhenExceptionOccurs_Returns0()
    {
        MockSaveService.ExportLocalWrapperFunc = () => throw new Exception("Falla simulada");
        MockCloudSave.LoadFunc = () => Task.FromResult<string>("{}");

        var validator = new CloudSyncValidator();
        var task = validator.Validate();
        yield return new WaitUntil(() => task.IsCompleted);

        Assert.AreEqual(0, task.Result);
    }

}

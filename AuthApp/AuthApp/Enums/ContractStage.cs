using AuthApp.Enums;

namespace AuthApp.Enums
{
    public enum ContractStage
    {
        Created,                       // Создан
        OnApproval,                    // На согласовании
        ApprovedPendingAcknowledgement, // Согласован, на ознакомлении
        InProgress,                    // Выполняется
        Completed,                     // Завершён
        Terminated,                    // Расторгнут
        OnRegistration                 // На регистрации
    }
}

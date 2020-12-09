function mockFormNotifications(context) {
    context.getFormContext().ui.setFormNotification("A mock info form notification", "INFO", "info_notification");
    context.getFormContext().ui.setFormNotification("A mock warning form notification", "WARNING", "warning_notification");
    context.getFormContext().ui.setFormNotification("A mock error form notification", "ERROR", "error_notification");
}
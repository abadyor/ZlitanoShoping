namespace ClientUI.wwwroot.js
{
    public class site
    {
      function customAlertWithButtons(message, dotNetRef, onRegisterMethod, onOkMethod) {
        const modal = document.createElement('div');
        modal.style.position = 'fixed';
        modal.style.top = '0';
        modal.style.left = '0';
        modal.style.width = '100%';
        modal.style.height = '100%';
        modal.style.backgroundColor = 'rgba(0, 0, 0, 0.5)';
        modal.style.display = 'flex';
        modal.style.justifyContent = 'center';
        modal.style.alignItems = 'center';
        modal.style.zIndex = '9999';

        const dialog = document.createElement('div');
        dialog.style.backgroundColor = '#fff';
        dialog.style.padding = '20px';
        dialog.style.borderRadius = '8px';
        dialog.style.textAlign = 'center';

        const text = document.createElement('p');
        text.innerText = message;
        dialog.appendChild(text);

        const registerButton = document.createElement('button');
        registerButton.innerText = 'Register';
        registerButton.style.margin = '10px';
        registerButton.style.padding = '10px 20px';
        registerButton.style.backgroundColor = '#007bff';
        registerButton.style.color = '#fff';
        registerButton.style.border = 'none';
        registerButton.style.borderRadius = '4px';
        registerButton.style.cursor = 'pointer';
        registerButton.onclick = () => {
            modal.remove();
            dotNetRef.invokeMethodAsync(onRegisterMethod);
        };

        const okButton = document.createElement('button');
        okButton.innerText = 'OK';
        okButton.style.margin = '10px';
        okButton.style.padding = '10px 20px';
        okButton.style.backgroundColor = '#28a745';
        okButton.style.color = '#fff';
        okButton.style.border = 'none';
        okButton.style.borderRadius = '4px';
        okButton.style.cursor = 'pointer';
        okButton.onclick = () => {
            modal.remove();
            dotNetRef.invokeMethodAsync(onOkMethod);
        };

        dialog.appendChild(registerButton);
        dialog.appendChild(okButton);
        modal.appendChild(dialog);
        document.body.appendChild(modal);
    }


    }
}

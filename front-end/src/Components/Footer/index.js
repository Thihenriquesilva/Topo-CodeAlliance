import React from 'react';

import './style.css';

export default function Footer(){
    return (
        <footer className="between">
            <div>
                <p>Tranquilo Jobs - Desenvolvido por Code Alliance</p>
                <p>Copyright 2021 © Todos os direitos reservados.</p>
            </div>
         <div>
           <p className="text-end">Hackathon on-line - Shawee</p>
           <p className="text-end">R. Dr.Alfredo de Castro,200 - Barra Funda - São Paulo-SP, 01155-060</p>
           <p className="text-end">Telefone: (11) 3273-5000 |E-mail:Osbrabos@gmail.com</p>
         </div>
       </footer>
    );
}
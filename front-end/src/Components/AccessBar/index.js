import React from 'react';
import { Link } from 'react-router-dom';

import './style.css';



export default function AccessBar() {
    return(
        <section className="barra">
            <div className="flex between">
                <div className="flex">
                    <p className="links"><a href="#menu" aria-label="Tecla de acesso para navegar no menu" className="topbar" accessKey="1"><b>1</b> Menu</a></p>
                    <p className="links"><a href="#conteudo" aria-label="Tecla de acesso para conteúdo" className="topbar" accessKey="2"><b>2</b> Conteúdo</a></p>
                    <p className="links"><Link to="#bar" aria-label="Tecla de acesso para menu de acessibilidade" accessKey="A" className="topbar" id="btnbar1"><b>A</b> Menu de acessibilidade</Link></p>
                </div>

                <div className="flex">
                    <p className="links"><a href="https://www.instagram.com/senai_info/" aria-label="Instagram do SENAI Informática" className="topbar fa fa-instagram topicon" target="_black"></a></p>
                </div>
            </div>
        </section>
    );
}
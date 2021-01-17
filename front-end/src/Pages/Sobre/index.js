import React from "react";

import Header from "../../Components/Header";
import Footer from "../../Components/Footer";
import AccessBar from "../../Components/AccessBar";
import AccessMenu from "../../Components/AccessMenu";





import "./style.css";

export default function Sobre() {
  return (
    <body className="corpo">
      <AccessBar />
      <Header />
      <AccessMenu />
      <div className="sobrePage">
        <h1 className="sobre">Sobre</h1>
      </div>
      <Footer />
    </body>
  );
}

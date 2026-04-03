<%@ Page Title="" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PRAJ_Report.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <!-- PARTICLE JS -->
    <script src="https://cdn.jsdelivr.net/npm/tsparticles@2/tsparticles.bundle.min.js"></script>

    <style>
        body {
            margin: 0;
            height: 100vh;
            overflow: hidden;
            font-family: Rockwell, serif;
            background: #0f2027;
        }

        /* PARTICLES BACKGROUND */
        #tsparticles {
            position: fixed;
            width: 100%;
            height: 100%;
            z-index: -1;
        }

        /* GLASS BOX */
        .glass-box {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            padding: 50px;
            width: 520px;
            text-align: center;
            border-radius: 15px;

            background: rgba(255, 255, 255, 0.1);
            backdrop-filter: blur(15px);
            box-shadow: 0 0 40px rgba(0,0,0,0.5);
        }

        /* LOGO */
        .logo {
            width: 120px;
            margin-bottom: 25px;
            animation: glow 2s infinite alternate;
        }

        @keyframes glow {
            from { filter: drop-shadow(0 0 5px #fff); }
            to { filter: drop-shadow(0 0 25px #00c6ff); }
        }

        /* TYPING TEXT */
        .typing-text {
            font-size: 34px;
            font-weight: 800;
            color: #ffffff;
            letter-spacing: 3px;
            text-align: center;
        }

        /* CURSOR */
        .cursor {
            display: inline-block;
            width: 3px;
            background: white;
            margin-left: 5px;
            animation: blink 0.7s infinite;
        }

        @keyframes blink {
            50% { opacity: 0; }
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="tsparticles"></div>

    <div class="glass-box">

        <img src="images/logo.png" class="logo" />

        <div class="typing-text" id="typing"></div>

    </div>

    <script>
        tsParticles.load("tsparticles", {
            particles: {
                number: { value: 60 },
                size: { value: 3 },
                move: { speed: 1 },
                links: {
                    enable: true,
                    distance: 150,
                    color: "#00c6ff"
                }
            }
        });
    </script>

    <script>
        const text = "WELCOME TO PRAJ INDUSTRIES";
        let index = 0;
        const speed = 100;

        function typeEffect() {
            if (index < text.length) {
                document.getElementById("typing").innerHTML += text.charAt(index);
                index++;
                setTimeout(typeEffect, speed);
            } else {
                document.getElementById("typing").innerHTML += '<span class="cursor"></span>';
            }
        }

        window.onload = typeEffect;
    </script>

</asp:Content>
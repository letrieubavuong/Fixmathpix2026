\NeedsTeXFormat{LaTeX2e}[1995/12/01]
\ProvidesClass{MAPClass}[2025/06/01 by LE TRIEU BA VUONG]
  \newcommand\@ptsize{}
  \newif\if@restonecol
  \newif\if@titlepage
  \@titlepagetrue
  \newif\if@openright
  \newif\if@mainmatter \@mainmattertrue
  \if@compatibility\else
  \DeclareOption{a4paper}
    {\setlength\paperheight {297mm}%
      \setlength\paperwidth  {210mm}}
  \DeclareOption{a5paper}
    {\setlength\paperheight {210mm}%
      \setlength\paperwidth  {148mm}}
  \DeclareOption{b5paper}
    {\setlength\paperheight {250mm}%
      \setlength\paperwidth  {176mm}}
  \DeclareOption{letterpaper}
    {\setlength\paperheight {11in}%
      \setlength\paperwidth  {8.5in}}
  \DeclareOption{legalpaper}
    {\setlength\paperheight {14in}%
      \setlength\paperwidth  {8.5in}}
  \DeclareOption{executivepaper}
    {\setlength\paperheight {10.5in}%
      \setlength\paperwidth  {7.25in}}
  \DeclareOption{landscape}
    {\setlength\@tempdima   {\paperheight}%
      \setlength\paperheight {\paperwidth}%
      \setlength\paperwidth  {\@tempdima}}
  \fi
  \if@compatibility
    \renewcommand\@ptsize{0}
  \else
  \DeclareOption{10pt}{\renewcommand\@ptsize{0}}
  \fi
  \DeclareOption{11pt}{\renewcommand\@ptsize{1}}
  \DeclareOption{12pt}{\renewcommand\@ptsize{2}}
  \if@compatibility\else
  \DeclareOption{oneside}{\@twosidefalse \@mparswitchfalse}
  \fi
  \DeclareOption{twoside}{\@twosidetrue  \@mparswitchtrue}
  \DeclareOption{draft}{\setlength\overfullrule{5pt}}
  \if@compatibility\else
  \DeclareOption{final}{\setlength\overfullrule{0pt}}
  \fi
  \DeclareOption{titlepage}{\@titlepagetrue}
  \if@compatibility\else
  \DeclareOption{notitlepage}{\@titlepagefalse}
  \fi
  \if@compatibility
  \@openrighttrue
  \else
  \DeclareOption{openright}{\@openrighttrue}
  \DeclareOption{openany}{\@openrightfalse}
  \fi
  \if@compatibility\else
  \DeclareOption{onecolumn}{\@twocolumnfalse}
  \fi
  \DeclareOption{twocolumn}{\@twocolumntrue}
  \DeclareOption{leqno}{\input{leqno.clo}}
  \DeclareOption{fleqn}{\input{fleqn.clo}}
  \DeclareOption{openbib}{%
    \AtEndOfPackage{%
    \renewcommand\@openbib@code{%
        \advance\leftmargin\bibindent
        \itemindent -\bibindent
        \listparindent \itemindent
        \parsep \z@
        }%
    \renewcommand\newblock{\par}}%
  }
\ExecuteOptions{a4paper,12pt,twoside,onecolumn,final,openright}
\ProcessOptions
\input{bk1\@ptsize.clo}
%%%GÓI LỆNH VỀ FONT
  \RequirePackage{fouriernc}
  \RequirePackage{fix-cm} % Tăng kích thước chữ
  \newcommand\myfont[2]{\fontsize{#1}{#2}\selectfont}
  \RequirePackage{amsmath,amssymb, amsfonts, amssymb, latexsym, amscd}	
  \RequirePackage{mathrsfs}
  \RequirePackage{polynom}
  \RequirePackage{physics}
%TỰ ĐIỀU CHỈNH DẤU NGOẶC===========================
  \DeclareDocumentCommand\pbrace{l m }{\braces#1{(}{)}{#2}}
  \DeclareDocumentCommand\vbrace{l m }{\braces#1{\{}{\}}{#2}}
  \DeclareDocumentCommand\vlbrace{l m }{\braces#1{\{}{.}{#2}}
  \DeclareDocumentCommand\slbrace{l m }{\braces#1{[}{.}{#2}}
  \DeclareDocumentCommand\sbrace{l m }{\braces#1{[}{]}{#2}}
  \newenvironment{sqcases}{%
  \matrix@check\sqcases\env@sqcases
  }{%
  \endarray\right.%
  }
  \def\env@sqcases{%
  \let\@ifnextchar\new@ifnextchar
  \left\lbrack
  \def\arraystretch{1.2}%
  \array{@{}l@{\quad}l@{}}%
  }
  \let\@afterindentfalse\@afterindenttrue
  \@afterindentfalse
  \let\p\par
  \RequirePackage[utf8]{vietnam}
  \RequirePackage{tkz-tab}
  \RequirePackage[version=4]{mhchem} 
%%%%Gói lệnh vẽ hình
  \RequirePackage[most]{tcolorbox}
  \tcbuselibrary{skins,theorems, breakable, raster}
  \RequirePackage{tikzpagenodes}
  \RequirePackage{tikz}
  \usetikzlibrary{%
    arrows, backgrounds, calc, shapes, shapes.symbols, shapes.misc,% wg. rounded rectangle
    shapes.arrows,%
    arrows.meta,%
    shapes.geometric,%
    snakes,%
    shadows,%
    spy,
    through,%
    trees,
    chains,%
    intersections,%
    matrix,%
    patterns,
    positioning,% wg. " of "
    plotmarks,
    petri,
    quotes,%
    angles,%
    scopes,%
    decorations,
    decorations.pathmorphing,% /pgf/decoration/random steps | erste Graphik
    decorations.pathreplacing,%
    decorations.markings,
    decorations.text,%
  }

  \RequirePackage{tkz-euclide}
%  \usetkzobj{all}
  \RequirePackage{tikz-3dplot}
  \RequirePackage{tabvar}
  \RequirePackage{contour}
  \RequirePackage{pgfplots}
  \tikzstyle{mybox} = [draw=red, very thick,rectangle, rounded corners, inner sep=10pt, inner ysep=20pt]
  \tikzstyle{fancytitle} =[fill=blue, text=white]
  \usepgfplotslibrary{fillbetween}
  \tikzset{arrow data/.style 2 args={%
  decoration={%
  markings,mark=at position #1 with \arrow{#2}},postaction=decorate}
  }%  
  \tikzstyle directed=[postaction={decorate,decoration={markings,
  mark=at position .5 with {\arrow{>}}}}]
  \tikzstyle undirected=[postaction={decorate,decoration={markings,
  mark=at position .5 with {\arrow{<}}}}]
  \tikzset{
  >={Latex[width=2mm,length=2mm]},
  base/.style = {
    rectangle,
    rounded corners,
    draw=black,
    minimum width=1cm,
    minimum height=1cm,
    text centered,
    drop shadow={shadow xshift=0.6mm, shadow yshift=-0.6mm}
  },
  Style1/.style = {base, fill=Mapcolor, draw=Mapcolor},
  Style2/.style = {base, fill=Mapcolor!50, draw=Mapcolor!50},
  Style3/.style = {base, fill=Mapcolor!20, draw=Mapcolor!20},
  Style4/.style = {base, minimum width=2.5cm, fill=orange!15, draw=orange},
}
  \tikzset{cross/.style={cross out, draw=black, minimum size=2.5*(#1-\pgflinewidth), inner sep=0pt, outer sep=0pt},
  %default radius will be 1pt. 
  cross/.default={1pt}}		 
  \tikzset{hbox/.style={anchor=west,draw, rectangle,right,minimum height = 0.4cm,minimum width=1cm,label={[label distance=-4pt]$#1$}}} 
  \tikzset{vbox/.style={anchor=north,draw, rectangle,below,minimum height = 1cm,minimum width=0.4cm,label={[label distance=-3pt]right:$#1$}}} 
  \tikzset{dot/.style={circle=(1.5pt),inner sep=1.5pt,fill,label={[label distance=-2pt]#1},name=#1}}
  \tikzset{ampe/.style={anchor=center,draw, circle,fill=white,inner sep=0pt,
  text width=6mm,align=center,label={[label distance=-3pt]\ce{#1}}}}
  \tikzset{partial ellipse/.style args={#1:#2:#3}{insert path={+ (#1:#3) arc (#1:#2:#3)}}}
  \tikzset{lpin/.style={shape=circle,inner sep=+0pt,minimum size=+1.2mm,label={[label distance=-3pt]left:{#1}},fill=black}}            
  \tikzset{rpin/.style={shape=circle,inner sep=+0pt,minimum size=+1.2mm,label={[label distance=-3pt]right:{#1}},fill=black}}            
  \tikzset{apin/.style={shape=circle,inner sep=+0.1pt,minimum size=+1.2mm,label={[label distance=-4pt]above:{\myfont{12}{16}#1}},fill=black}}            
  \tikzset{bpin/.style={shape=circle,inner sep=+0pt,minimum size=+1.2mm,label={[label distance=-5pt]below:{#1}},fill=black}}            
  \tikzstyle{nonamcham}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
        {
        \draw[fill=white] (0,0)--(0.6,0.2)--(1.2,0)--(0.6,-0.2)--cycle;
        \draw[fill=white] (0,0)--(0.6,0.2)--(1.2,0)--(0.6,-0.2)--cycle (0.6,0.2)--(0.6,-0.2);
  %\draw[pattern=north west lines] (0,0)--(0.6,0.2)--(0.6,-0.2)--cycle;
  }}}]
  \tikzstyle{D}=[postaction={decorate, decoration={markings, mark= at position 0.5 with {
  \draw[fill=white] (0,0)circle(2.5mm);
  \node[cross] at (0,0) {\color{white}{A}};
  \node[pos=-0.5,sloped,above=4pt] at (0,0) {\myfont{12.55}{16}$#1$};
  }}}]  
  \tikzstyle{L}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
        {
        \fill[white] (-0.5,-0.1) rectangle (0.5,0.15);
        \draw [sloped,snake=coil, segment amplitude=5pt,segment length=5pt] (-0.5,0) -- (0.5,0); 
        \node[pos=-0.5,sloped,align=center,label={[label distance=-1pt]\ce{#1}}] at (0,0) {};
  }}}]
  \tikzstyle{vL}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
        {
        \fill[white] (-0.5,-0.1) rectangle (0.5,0.15);
        \draw[->] (-0.4,-0.3)--(0.35,0.35);
        \draw [sloped,snake=coil, segment amplitude=5pt,segment length=5pt] (-0.5,0) -- (0.5,0); 
        \node[pos=-0.5,sloped,align=center,label={[label distance=-1pt]\ce{#1}}] at (0,0) {};
  }}}]                       
  \tikzstyle{K}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
  {
    \fill[white] (-0.2,-0.1) rectangle (0.2,0.15);   
    \draw (-0.2,0)--(0.2,0.15); 		
    \filldraw[black] (0.2,0) circle(1pt);
    \filldraw[black] (-0.2,0) circle(1pt); 
    \node[pos=-0.5,sloped,align=center,label={[label distance=-1pt]\ce{#1}}] at (0,0) {};
  }}}] 
  \def\cucA{A}
  \def\cucB{B}
  \tikzstyle{NU}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
  {
    %\draw (-0.2,0)--(0.2,0.2);
    \draw[white] (-0.3,0)--(0.3,0);  
    \draw(-0.3,0)--++(45:0.2);
    \draw(-0.3,0)--++(-135:0.2);
    \draw(0.3,0)--++(45:0.2);	
    \draw(0.3,0)--++(-135:0.2);
    \filldraw[black] (0.3,0) circle(1pt);
    \filldraw[black] (-0.3,0) circle(1pt); 
    \node[sloped,align=center,label={[label distance=-1pt]\cucB}] at (0.3,0) {};
    \node[sloped,align=center,label={[label distance=-1pt]\cucA}] at (-0.3,0) {};
    \node[sloped,align=center,label={[label distance=-18pt]$-$}] at (0.3,0) {};
    \node[sloped,align=center,label={[label distance=-18pt]$+$}] at (-0.3,0) {};
  }}}] 
  \tikzstyle{volt}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
        {
        \fill[white] (-0.075,-0.3) rectangle (0.075,0.3);
        \foreach \x in {-0.075} {\draw (\x,-0.25)--(\x,0.25);}
                \foreach \y in {0.075} {\draw (\y,0.1)--(\y,-0.1);} 
          \node[pos=-0.5,sloped,align=center,label={[label distance=-1pt]$#1$}] at (0,1) {};
  }}}]     
  \tikzstyle{avolt}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
        {
        \fill[white] (-0.075,-0.3) rectangle (0.075,0.3);
        \foreach \x in {-0.075} {\draw (\x,-0.1)--(\x,0.1);}
                \foreach \y in {0.075} {\draw (\y,0.25)--(\y,-0.25);} 
          \node[pos=-0.5,sloped,align=center,label={[label distance=-1pt]$#1$}] at (0,1) {};
  }}}]          
  \tikzstyle{volt1}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
        {
        \fill[white] (-0.075,-0.3) rectangle (0.075,0.3);
        \foreach \x in {-0.075} {\draw (\x,-0.15)node[below left]{-}--(\x,0.15);}
                \foreach \y in {0.075} {\draw (\y,0.3)--(\y,-0.3)node[right]{+};} 
          \node[pos=-0.5,sloped,above=6pt] at (0,1) {$#1$};
  }}}]       
  \tikzstyle{volt2}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
        {
        \fill[white] (-0.225,-0.3) rectangle (0.225,0.3);
        \foreach \x in {-0.075,0.225} {
    `								\draw (\x,-0.15)--(\x,0.15);
                }
                \foreach \y in {-0.225,0.075} {
                    \draw (\y,0.3)--(\y,-0.3);
                } 
          \node[pos=-0.5,sloped,above=3pt] at (0,1) {$#1$};
  }}}] 
  \tikzstyle{volt3}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
        {
        \fill[white] (-0.4,-0.3) rectangle (0.4,0.3);
        \foreach \x in {-0.375,-0.075,0.225} {
    `								\draw (\x,-0.3)--(\x,0.3);}
                \foreach \y in {-0.225,0.075,0.375} {
                    \draw (\y,0.15)--(\y,-0.15);
                } 
          \node[pos=-0.5,align=center,label={[label distance=-1pt]$#1$}] at (0,1) {};
  }}}] 
  \tikzstyle{volt4}=[postaction={decorate, decoration={markings, mark= at position 0.5 with {
        \draw[white,line width=0.6mm,fill=white] (-0.525,0)--(0.525,0);     
        \foreach \x in {-0.525,-0.225,0.075,0.375} {
      \draw (\x,-0.25)--(\x,0.25);}
      \foreach \y in {-0.375,-0.075,0.225,0.525} {
      \draw (\y,0.1)--(\y,-0.1);}
          \node[pos=-0.5,align=center,label={[label distance=-3pt]$#1$}] at (0,1) {};
  }}}]                           
  \tikzstyle{volt5}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
        {
        \fill[white] (-0.675,-0.3) rectangle (0.675,0.3);
        \foreach \x in {-0.675,-0.375,-0.075,0.225,0.525} {
    `	\draw (\x,-0.25)--(\x,0.25);}
      \foreach \y in {-0.525,-0.225,0.075,0.375,0.525,0.675} {
      \draw (\y,0.1)--(\y,-0.1);} 
          \node[pos=-0.5,align=center,label={[label distance=-3pt]$#1$}] at (0,1) {};
  }}}] 
  \tikzstyle{voltn}=[postaction={decorate, decoration={markings, mark= at position 0.5 with{
  \fill[white] (-0.675,-0.05) rectangle (0.675,0.05);
  \foreach \x in {-0.675,-0.375,0.525} {
    \draw[very thick] (\x,-0.25)--(\x,0.25);
    \draw[very thick, dashed] (-0.225,0)--(0.525,0);
  }
  \foreach \y in {-0.525,-0.225,0.675} {\draw[very thick] (\y,0.1)--(\y,-0.1);} 
  \node[pos=-0.5,align=center,label={[label distance=-2pt]\myfont{12}{12}$#1$}] at (0,1) {};
  }}}]        
  \tikzstyle{rvR}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
    {
    \draw[->] (0,0.8)--(0,0.3);
    \draw (-0.02,0.8)--(1,0.8)--(1,0);	
    \fill[white] (0.5,-0.3) rectangle (0.978,0.1);	
    \node[right,sloped, draw, rectangle, fill=white, inner sep=0pt,
  minimum width=10mm, minimum height = 4mm,align=center,label={[label distance=-1pt]\ce{#1}}] at (-0.5,0) {};
  \node[right] at (-0.8,0.45) {\bth{R_b}};
    }}}]  
  \tikzstyle{lvR}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
    {
    \draw[->] (0.5,0.8)--(0.5,0.3);
    \draw (0.52,0.8)--(-0.5,0.8)--(-0.5,0);	
    \fill[white] (-0.478,-0.3) rectangle (0,0.1);	
    \node[right,sloped, draw, rectangle, fill=white, inner sep=0pt,
  minimum width=10mm, minimum height = 4mm,align=right] at (0,0) {};
  \node[right] at (0.4,0.45) {\bth{R_b}};
    }}}] 
  \tikzstyle{dvR}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
    {
    \clip(-0.7,-0.2) rectangle (1,1) ;
    \draw[->] (0.2,0.6)--(0.2,0.2);
    \draw (-0.7,0)--(-0.7,0.6)--(0.22,0.6);	
    \fill[white] (-0.678,-0.2) rectangle (0,0.1);	
    \node[sloped, draw, rectangle, fill=white, inner sep=0pt,
  minimum width=4mm, minimum height = 10mm] at (0.2,0) {};
  \node[sloped, above right=3pt] at (1,0) {\bth{R_b}};
    }}}] 
  % TỤ ĐIỆN
  \tikzstyle{C}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
        {
        \draw[white,line width=0.6mm,fill=white] (-0.1,0)--(0.1,0);
        \draw (-0.1,0.2) -- (-0.1,-0.2) (0.1,0.2) -- (0.1,-0.2);
          \node[pos=-0.5,sloped,above=3pt] at (0,1) {\ce{#1}};
  }}}] 
  \tikzstyle{vC}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
        {
        \draw[white,line width=0.6mm,fill=white] (-0.1,0)--(0.1,0);
        \draw (-0.1,0.2) -- (-0.1,-0.2) (0.1,0.2) -- (0.1,-0.2);
          \draw[->] (-0.4,-0.3)--(0.35,0.35);
          \node[pos=-0.5,sloped,above=3pt] at (0,1) {\ce{#1}};
  }}}]  
  %Bình điện phân
  \tikzstyle{M}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
        {
        \fill[white] (-0.1,-0.3) rectangle (0.1,0.3);
        %\draw (-0.1,0.15) -- (-0.1,-0.15) (0.1,0.15) -- (0.1,-0.15);
        \draw (0,0)circle(7pt);
          \node[pos=-0.5,sloped,above=3pt] at (0,1) {\ce{#1}};
  }}}]        
  % ĐIỆN TRỞ
  \tikzstyle{R}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
    {
    \node[pos=0.5,sloped, draw, rectangle, fill=white, inner sep=0pt, minimum width=10mm, minimum height = 4mm,align=center,label={[label distance=-1pt]\ce{#1}}] at (0,0) {};
    }}}]
  \tikzstyle{RX}=[
  postaction={decorate, decoration={markings, mark= at position 0.5 with
  {
    \node[pos=0.5,sloped, draw, rectangle, fill=white, inner sep=0pt,
    minimum width=10mm, minimum height = 6mm,align=center,label={[label 		distance=-3pt]$#1$}] at (0,0) {X};
  }}}]  
  \tikzstyle{vR}=[postaction={decorate, decoration={markings, mark= at position 0.5 with {\node[pos=0.5,sloped, draw, rectangle, fill=white, inner sep=0pt,
  minimum width=10mm, minimum height = 4mm,align=center,label={[label distance=-3pt]\ce{#1}}] at (0,0) {};
    \draw[->] (-0.4,-0.3)--(0.35,0.35);
  }}}]  
  \tikzstyle{xR}=[postaction={decorate, decoration={markings, mark= at position 0.5 with {\node[pos=0.5,sloped, draw, rectangle, fill=white, inner sep=0pt,
  minimum width=10mm, minimum height = 4mm,align=center,label={[label distance=-1pt]\ce{#1}}] at (0,0) {};
    \draw[-] (-0.5,-0.2)--(0.5,0.2);
  }}}]  
  %AMPE KẾ
  \tikzstyle{A}=[postaction={decorate, decoration={markings, mark= at position 0.5 with {\node[pos=0.5, draw, circle, fill=white, inner sep=0pt,
  minimum width=6mm, minimum height = 6mm,align=center,label={[label distance=-3pt]}] at (0,0) {\ce{A#1}};
  }}}]  
  \tikzstyle{V}=[postaction={decorate, decoration={markings, mark= at position 0.5 with {\node[pos=0.5,sloped, draw, circle, fill=white, inner sep=0pt,
  minimum width=6mm, minimum height = 6mm,align=center,label={[label distance=-3pt]}] at (0,0) {\ce{V#1}};
  }}}] 
  \tikzstyle{MD}=[postaction={decorate, decoration={markings, mark= at position 0.5 with {\node[pos=0.5,sloped, draw, circle, fill=white, inner sep=0pt,
  minimum width=6mm, minimum height = 6mm,align=center,label={[label distance=-6mm]\ce{M#1}}] at (0,0) {};
  }}}]  
  \tikzstyle{DP}=[postaction={decorate, decoration={markings, mark= at position 0.5 with {\node[pos=0.5,sloped, draw, circle, fill=white, inner sep=0pt,
  minimum width=6mm, minimum height = 6mm,align=center,label={[label distance=-3pt]\ce{#1}}] at (0,0) {\normalsize{\ce{| |}}};
  }}}] 
  % VÔN KẾ
  \tikzset{lpin/.style={shape=circle,inner sep=+0pt,minimum size=+1.2mm,label={[label distance=-3pt]left:{#1}},fill=black}}            
  \tikzset{rpin/.style={shape=circle,inner sep=+0pt,minimum size=+1.2mm,label={[label distance=-3pt]right:{#1}},fill=black}}            
  \tikzset{apin/.style={shape=circle,inner sep=+0.1pt,minimum size=+1.2mm,label={[label distance=-3pt]above:{\myfont{12}{16}#1}},fill=black}}            
  \tikzset{bpin/.style={shape=circle,inner sep=+0pt,minimum size=+1.2mm,label={[label distance=-3pt]below:{#1}},fill=black}}            
  \newenvironment{machdien}[1]{%
        \centering\begin{tikzpicture}[scale=1,very thick,>=stealth']
        #1
        }{\end{tikzpicture} }	
  \newcommand{\apin}[1]{node[apin=#1]{}}
  \newcommand{\bpin}[1]{node[bpin=$#1$]{}}
  \newcommand{\lpin}[1]{node[lpin=#1]{}}
  \newcommand{\rpin}[1]{node[rpin=#1]{}}
  \tikzset{ hidden/.style = {thick, dashed}}
  \tikzset{line/.style = {ultra thick}}
  \tkzSetUpPoint[fill=black,size=6]
  \tikzstyle{mybox} = [draw=red, very thick,
  rectangle, rounded corners, inner sep=10pt, inner ysep=20pt]
  \tikzstyle{fancytitle} =[fill=blue, text=white]
  \RequirePackage{nicefrac}
  \RequirePackage{parallel}
  \tikzset{arrow data/.style 2 args={%
  decoration={%
    markings,mark=at position #1 with \arrow{#2}},postaction=decorate}
  }%

%%%MẶC ĐỊNH KHÔNG CHỈNH SỬA
  \setlength\lineskip{1\p@}
  \setlength\normallineskip{1\p@}
  \renewcommand\baselinestretch{}
  \setlength\parskip{0\p@ \@plus \p@}
  \@lowpenalty   51
  \@medpenalty  151
  \@highpenalty 301
  \setcounter{topnumber}{2}
  \renewcommand\topfraction{.7}
  \setcounter{bottomnumber}{1}
  \renewcommand\bottomfraction{.3}
  \setcounter{totalnumber}{3}
  \renewcommand\textfraction{.2}
  \renewcommand\floatpagefraction{.5}
  \setcounter{dbltopnumber}{2}
  \renewcommand\dbltopfraction{.7}
  \renewcommand\dblfloatpagefraction{.5}
  \if@twoside
    \def\ps@headings{%
        \let\@oddfoot\@empty\let\@evenfoot\@empty
        \def\@evenhead{\thepage\hfil\slshape\leftmark}%
        \def\@oddhead{{\slshape\rightmark}\hfil\thepage}%
        \let\@mkboth\markboth
      \def\chaptermark##1{%
        \markboth {\MakeUppercase{%
          \ifnum \c@secnumdepth >\m@ne
            \if@mainmatter
              \@chapapp\ \thechapter. \ %
            \fi
          \fi
          ##1}}{}}%
      \def\sectionmark##1{%
        \markright {\MakeUppercase{%
          \ifnum \c@secnumdepth >\z@
            \thesection. \ %
          \fi
          ##1}}}}
  \else
    \def\ps@headings{%
      \let\@oddfoot\@empty
      \def\@oddhead{{\slshape\rightmark}\hfil\thepage}%
      \let\@mkboth\markboth
      \def\chaptermark##1{%
        \markright {\MakeUppercase{%
          \ifnum \c@secnumdepth >\m@ne
            \if@mainmatter
              \@chapapp\ \thechapter. \ %
            \fi
          \fi
          ##1}}}}
  \fi
  \def\ps@myheadings{%
      \let\@oddfoot\@empty\let\@evenfoot\@empty
      \def\@evenhead{\thepage\hfil\slshape\leftmark}%
      \def\@oddhead{{\slshape\rightmark}\hfil\thepage}%
      \let\@mkboth\@gobbletwo
      \let\chaptermark\@gobble
      \let\sectionmark\@gobble
      }
    \if@titlepage
    \newcommand\maketitle{\begin{titlepage}%
    \let\footnotesize\small
    \let\footnoterule\relax
    \let \footnote \thanks
    \null\vfil
    \vskip 60\p@
    \begin{center}%
      {\LARGE \@title \par}%
      \vskip 3em%
      {\large
      \lineskip .75em%
        \begin{tabular}[t]{c}%
          \@author
        \end{tabular}\par}%
        \vskip 1.5em%
      {\large \@date \par}%       % Set date in \large size.
    \end{center}\par
    \@thanks
    \vfil\null
    \end{titlepage}%
    \setcounter{footnote}{0}%
    \global\let\thanks\relax
    \global\let\maketitle\relax
    \global\let\@thanks\@empty
    \global\let\@author\@empty
    \global\let\@date\@empty
    \global\let\@title\@empty
    \global\let\title\relax
    \global\let\author\relax
    \global\let\date\relax
    \global\let\and\relax
  }
  \else
  \newcommand\maketitle{\par
    \begingroup
      \renewcommand\thefootnote{\@fnsymbol\c@footnote}%
      \def\@makefnmark{\rlap{\@textsuperscript{\normalfont\@thefnmark}}}%
      \long\def\@makefntext##1{\parindent 1em\noindent
              \hb@xt@1.8em{%
                  \hss\@textsuperscript{\normalfont\@thefnmark}}##1}%
      \if@twocolumn
        \ifnum \col@number=\@ne
          \@maketitle
        \else
          \twocolumn[\@maketitle]%
        \fi
      \else
        \newpage
        \global\@topnum\z@   % Prevents figures from going at top of page.
        \@maketitle
      \fi
      \thispagestyle{plain}\@thanks
    \endgroup
    \setcounter{footnote}{0}%
    \global\let\thanks\relax
    \global\let\maketitle\relax
    \global\let\@maketitle\relax
    \global\let\@thanks\@empty
    \global\let\@author\@empty
    \global\let\@date\@empty
    \global\let\@title\@empty
    \global\let\title\relax
    \global\let\author\relax
    \global\let\date\relax
    \global\let\and\relax
  }
  \def\@maketitle{%
    \newpage
    \null
    \vskip 2em%
    \begin{center}%
    \let \footnote \thanks
      {\LARGE \@title \par}%
      \vskip 1.5em%
      {\large
        \lineskip .5em%
        \begin{tabular}[t]{c}%
          \@author
        \end{tabular}\par}%
      \vskip 1em%
      {\large \@date}%
    \end{center}%
    \par
    \vskip 1.5em}
  \fi
%%%ĐIỀU CHỈNH SECTION
  \newcommand*\chaptermark[1]{}
  \setcounter{secnumdepth}{4}
  \newcounter {part}
  \newcounter {chapter}
  \newcounter {section}[chapter]
  \newcounter {subsection}[section]
  \newcounter {subsubsection}[subsection]
  \newcounter {paragraph}[subsubsection]
  \newcounter {subparagraph}[paragraph]
  \renewcommand \thepart {\@Roman\c@part}
  \renewcommand \thechapter {\@arabic\c@chapter}
  \renewcommand \thesection {\@arabic\c@section}
  \renewcommand\thesubsection   {\@Roman\c@subsection}
  \renewcommand\thesubsubsection{\@arabic\c@subsubsection}
  \renewcommand\theparagraph    {\@alph\c@paragraph}
  \renewcommand\thesubparagraph {\theparagraph.\@arabic\c@subparagraph}
  \newcommand\@chapapp{\chaptername}
  \newcommand\frontmatter{%
      \cleardoublepage
    \@mainmatterfalse
    \pagenumbering{roman}}
  \newcommand\mainmatter{%
      \cleardoublepage
    \@mainmattertrue
    \pagenumbering{arabic}}
  \newcommand\backmatter{%
    \if@openright
      \clearpage
    \else
      \clearpage
    \fi
    \@mainmatterfalse}
  \newcommand\part{%
    \if@openright
      \clearpage
    \else
      \clearpage
    \fi    
    \if@twocolumn
      \onecolumn
      \@tempswatrue
    \else
      \@tempswafalse
    \fi
    \thispagestyle{fancy}%
    \null\vfil
    \secdef\@part\@spart}

  \def\@part[#1]#2{%
      \ifnum \c@secnumdepth >-2\relax
        \refstepcounter{part}%
        \addcontentsline{toc}{part}{\thepart.\hspace{1em}#1}%
      \else
        \addcontentsline{toc}{part}{\thepart. #1}%
      \fi
      \markboth{}{}%
      {\centering
      \interlinepenalty \@M
      \normalfont
      \ifnum \c@secnumdepth >-2\relax
        \huge\bfseries \partname\nobreakspace\thepart.
        \par
        \vskip 20\p@
      \fi
      \Huge \bfseries #2\par}%
      \@endpart}
  \def\@spart#1{%
      {\centering
      \interlinepenalty \@M
      \normalfont
      \Huge \bfseries #1\par}%
      \@endpart}
  \def\@endpart{\vfil\clearpage
                \if@twoside
                  \if@openright
                    \thispagestyle{fancy}%
                  \fi
                \fi
                \if@tempswa
                  \twocolumn
                \fi
                }
  \newcommand\chapter{\if@openright\cleardoublepage\else\clearpage\fi
                      \thispagestyle{fancy}%
                      \global\@topnum\z@
                      \@afterindentfalse
                      \secdef\@chapter\@schapter}
  \def\@chapter[#1]#2{\ifnum \c@secnumdepth >\m@ne
                        \if@mainmatter
                          \refstepcounter{chapter}%
                          \typeout{\@chapapp\space\thechapter.}%
                          \addcontentsline{toc}{chapter}%
                                    {\protect\numberline{\thechapter}#1}%
                        \else
                          \addcontentsline{toc}{chapter}{#1}%
                        \fi
                      \else
                        \addcontentsline{toc}{chapter}{#1}%
                      \fi
                      \chaptermark{#1}%
                      \addtocontents{lof}{\protect\addvspace{10\p@}}%
                      \addtocontents{lot}{\protect\addvspace{10\p@}}%
                      \if@twocolumn
                        \@topnewpage[\@makechapterhead{#2}]%
                      \else
                        \@makechapterhead{#2}%
                        \@afterheading
                      \fi}
  \def\@makechapterhead#1{%
    \vspace*{50\p@}%
    {\parindent \z@ \raggedright \normalfont
      \ifnum \c@secnumdepth >\m@ne
        \if@mainmatter
          \huge\bfseries \@chapapp\space \thechapter
          \par\nobreak
          \vskip 20\p@
        \fi
      \fi
      \interlinepenalty\@M
      \Huge \bfseries #1\par\nobreak
      \vskip 40\p@
    }}
  \def\@schapter#1{\if@twocolumn
                    \@topnewpage[\@makeschapterhead{#1}]%
                  \else
                    \@makeschapterhead{#1}%
                    \@afterheading
                  \fi}
  \def\@makeschapterhead#1{%
    \vspace*{50\p@}%
    {\parindent \z@ \raggedright
      \normalfont
      \interlinepenalty\@M
      \Huge \bfseries  #1\par\nobreak
      \vskip 40\p@
    }}
    \newcommand\section{\@startsection{section}{1}{\z@}%
    {-2pt}%
    {2pt}%
    {\normalfont\Large\bfseries}}
  \newcommand\subsection{\@startsection{subsection}{2}{\z@}%
    {-1pt}%
    {2pt}%
    {\normalfont\Large\bfseries\fontfamily{qag}\selectfont}}
  \newcommand\subsubsection{\@startsection{subsubsection}{3}{\z@}%
    {-2pt}%
    {2pt}%
    {\normalfont\normalsize\bfseries}}
  \newcommand\paragraph{\@startsection{paragraph}{4}{\z@}%
    {-2pt}%
    {2pt}%
    {\normalfont\normalsize\bfseries}}
  \newcommand\subparagraph{\@startsection{subparagraph}{5}{\parindent}%
    {-2pt}%
    {2pt}%
    {normalfont\normalsize\bfseries}}
  \if@twocolumn
    \setlength\leftmargini  {2em}
  \else
    \setlength\leftmargini  {2.5em}
  \fi
  \leftmargin  \leftmargini
  \setlength\leftmarginii  {2.2em}
  \setlength\leftmarginiii {1.87em}
  \setlength\leftmarginiv  {1.7em}
  \if@twocolumn
    \setlength\leftmarginv  {.5em}
    \setlength\leftmarginvi {.5em}
  \else
    \setlength\leftmarginv  {1em}
    \setlength\leftmarginvi {1em}
  \fi
%%%%ĐỊNH NGHĨA LẠI MÀU SẮC
  \definecolor{Mapcolor}{cmyk}{0, 0.948, 0.948, 0.502}
  \definecolor{coolblack}{cmyk}{1.0, 0.538, 0.0, 0.61}
  \definecolor{colorA}{cmyk}{0.78, 0.46, 0.0, 0.06}
  \definecolor{colorB}{cmyk}{0.0, 0.72, 0.80, 0.11}
  \definecolor{colorC}{cmyk}{0.70, 0.0, 0.55, 0.36}
  \definecolor{colorD}{cmyk}{0.0, 0.68, 0.77, 0.09}
  \definecolor{colorE}{cmyk}{0.64, 0.0, 0.29, 0.58}
  \definecolor{colorF}{cmyk}{0.92, 0.37, 0.0, 0.27}
  \definecolor{colorG}{cmyk}{1.0, 0.22, 0.0, 0.14}  
  \definecolor{colorH}{cmyk}{0.0, 1.0, 0.85, 0.41}
  \definecolor{colorY}{cmyk}{0.0, 0.84, 0.84, 0.3}
  \definecolor{colorK}{cmyk}{1.0, 0.26, 0.0, 0.35}
  \definecolor{colorL}{cmyk}{0.0, 0.58, 0.58, 0.57}
  \definecolor{colorM}{cmyk}{0.88, 0.40, 0.0, 0.52}
  \definecolor{colorN}{cmyk}{0.0, 0.47, 0.87, 0.03}
  \newcommand{\Logomap}{
    \draw[Mapcolor,line width=2pt,rounded corners = 1pt,fill=Mapcolor] (0,-0.2) rectangle (0.45,0.2);
    \draw[white,line width=2pt] (0.05,-0.2)--++(90:0.3)--++(-45:0.2)--++(45:0.2)--++(-90:0.2)coordinate(b);
    \draw[white,line width=2pt] ($(b)-(0.035,0.06)$)--++(0:0.15);
  }
  \RequirePackage{pgfmath}
  \newsavebox{\Randomsymbol} 
  \begin{lrbox}{\Randomsymbol}
    \begin{tikzpicture}
      % Kích thước vùng vẽ: 17cm x 24cm
      \def\w{17}  % Chiều rộng vùng vẽ
      \def\h{24}  % Chiều cao vùng vẽ
      % Định nghĩa các từ trong danh sách
      \def\wordA{HÀM SỐ}
      \def\wordB{đồ thị}
      \def\wordC{biến thiên}
      \def\wordD{tiệm cận}
      \def\wordE{đạo hàm}
      \def\wordF{giới hạn}
      \def\wordG{liên tục}
      \def\wordH{cực trị}
      \def\wordI{đồng biến}
      \def\wordK{nghịch biến}
      % Danh sách màu
      \definecolor{colorone}{rgb}{0.8, 0.2, 0.2} % Đỏ
      \definecolor{colortwo}{rgb}{0.2, 0.8, 0.2} % Xanh lá
      \definecolor{colorthree}{rgb}{0.2, 0.2, 0.8} % Xanh dương
      \definecolor{colorfour}{rgb}{0.8, 0.8, 0.2} % Vàng
      \definecolor{colorfive}{rgb}{0.8, 0.4, 0.2} % Cam
      \definecolor{colorsix}{rgb}{0.2, 0.8, 0.8} % Xanh ngọc
      \definecolor{colorseven}{rgb}{0.8, 0.8, 0.8} % Xám
      % Số lượng từ muốn rải
      \def\n{55}
      % Lặp để rải từ ngẫu nhiên trong phạm vi vùng vẽ
      \foreach \i in {1,...,\n} {
        % Chọn ngẫu nhiên 1 chỉ mục từ 1 đến 10
        \pgfmathrandominteger{\index}{1}{10}
        % Chọn từ ngẫu nhiên
        \def\myword{ }
        \ifnum\index=1 \def\myword{\wordA} \fi
        \ifnum\index=2 \def\myword{\wordB} \fi
        \ifnum\index=3 \def\myword{\wordC} \fi
        \ifnum\index=4 \def\myword{\wordD} \fi
        \ifnum\index=5 \def\myword{\wordE} \fi
        \ifnum\index=6 \def\myword{\wordF} \fi
        \ifnum\index=7 \def\myword{\wordG} \fi
        \ifnum\index=8 \def\myword{\wordH} \fi
        \ifnum\index=9 \def\myword{\wordI} \fi
        \ifnum\index=10 \def\myword{\wordK} \fi
        % Chọn ngẫu nhiên màu từ danh sách
        \pgfmathrandominteger{\colorindex}{1}{7} % Chọn màu ngẫu nhiên từ 1 đến 7
        % Xác định màu ngẫu nhiên
        \def\mycolor{ }
        \ifnum\colorindex=1 \def\mycolor{colorone} \fi
        \ifnum\colorindex=2 \def\mycolor{colortwo} \fi
        \ifnum\colorindex=3 \def\mycolor{colorthree} \fi
        \ifnum\colorindex=4 \def\mycolor{colorfour} \fi
        \ifnum\colorindex=5 \def\mycolor{colorfive} \fi
        \ifnum\colorindex=6 \def\mycolor{colorsix} \fi
        \ifnum\colorindex=7 \def\mycolor{colorseven} \fi

        % Tạo vị trí ngẫu nhiên trong vùng vẽ (17cm x 24cm)
        \pgfmathsetmacro{\x}{rnd*\w}
        \pgfmathsetmacro{\y}{rnd*\h}

        % Chọn kích thước và độ mờ ngẫu nhiên
        \pgfmathsetmacro{\size}{12 + rnd*5} % Cỡ chữ từ 10pt đến 20pt
        \pgfmathsetmacro{\opacity}{0.4 + rnd*0.5} % Độ mờ từ 0.3 đến 1

        % Chọn góc xoay ngẫu nhiên từ -15 đến 15 độ
        \pgfmathsetmacro{\angle}{rnd*0}

        % Hiển thị từ ngẫu nhiên với màu ngẫu nhiên
        \node[opacity=\opacity, scale=\size/10, rotate=\angle, text=\mycolor] at (\x+0.6,\y+0.6) {\myword};
      }
    \end{tikzpicture}
  \end{lrbox}
  \newsavebox{\oldsqsymbol}  
  \newcommand{\Chonmau}[1]{
    \ifnum#1=01
      \colorlet{Mapcolor}{colorA}
      \definecolor{ViduG}{cmyk}{0, 0.45, 0.9, 0.0} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0, 0.7, 0.5, 0.0} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
      \end{tikzpicture}
      }
    \else\ifnum#1=02
      \colorlet{Mapcolor}{colorB}
      \definecolor{ViduG}{cmyk}{0, 0.45, 0.9, 0.0} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0, 0.7, 0.5, 0.0} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
      \end{tikzpicture}
      }
    \else\ifnum#1=03
      \colorlet{Mapcolor}{colorC}
      \definecolor{ViduG}{cmyk}{0, 0.45, 0.9, 0.0} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0, 0.7, 0.5, 0.0} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
      \end{tikzpicture}
      }
    \else\ifnum#1=04
      \colorlet{Mapcolor}{colorD}
      \definecolor{ViduG}{cmyk}{0, 0.45, 0.9, 0.0} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0, 0.7, 0.5, 0.0} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
      }
    \else\ifnum#1=05
      \colorlet{Mapcolor}{colorE}
      \definecolor{ViduG}{cmyk}{0.0, 0.55, 1.0, 0.15} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0.0, 0.55, 1.0, 0.15} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
      \end{tikzpicture}
      }
    \else\ifnum#1=06
      \colorlet{Mapcolor}{colorF}
      \definecolor{ViduG}{cmyk}{0, 0.45, 0.9, 0.0} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0, 0.7, 0.5, 0.0} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
      \end{tikzpicture}
      }
    \else\ifnum#1=07
      \colorlet{Mapcolor}{colorG}
      \colorlet{ViduG}{Mapcolor!30}
      \definecolor{ViduG}{cmyk}{0, 0.45, 0.9, 0.0} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0, 0.7, 0.5, 0.0} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
      \end{tikzpicture}
      }
    \else\ifnum#1=08
      \colorlet{Mapcolor}{colorH}
      \definecolor{ViduG}{cmyk}{0, 0.45, 0.9, 0.0} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0, 0.7, 0.5, 0.0} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
		\end{tikzpicture}
      }
    \else\ifnum#1=09
      \colorlet{Mapcolor}{colorY}
      \definecolor{ViduG}{cmyk}{0, 0.45, 0.9, 0.0} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0, 0.7, 0.5, 0.0} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
		\end{tikzpicture}
      }
    \else\ifnum#1=10
      \colorlet{Mapcolor}{colorK} 
      \definecolor{ViduG}{cmyk}{0, 0.45, 0.9, 0.0} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0, 0.7, 0.5, 0.0} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
      \end{tikzpicture}
      }
    \else\ifnum#1=11
      \colorlet{Mapcolor}{colorL} 
      \definecolor{ViduG}{cmyk}{0, 0.45, 0.9, 0.0} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0, 0.7, 0.5, 0.0} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
      \end{tikzpicture}
      }
    \else\ifnum#1=12
      \colorlet{Mapcolor}{colorM} 
      \definecolor{ViduG}{cmyk}{0, 0.45, 0.9, 0.0} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0, 0.7, 0.5, 0.0} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
      \end{tikzpicture}
      }
    \else\ifnum#1=13
      \colorlet{Mapcolor}{colorN} 
      \definecolor{ViduG}{cmyk}{0, 0.45, 0.9, 0.0} % Cam sáng
      \definecolor{BaitapG}{cmyk}{0, 0.7, 0.5, 0.0} % Hồng cam
	  \sbox{\oldsqsymbol}{%
      \begin{tikzpicture}[baseline=-0.8ex]
        \Logomap
      \end{tikzpicture}
      }
    \else
      \PackageError{myclass}{Mã màu bạn chọn không hợp lệ}
    \fi\fi\fi\fi\fi\fi\fi\fi\fi\fi\fi\fi\fi
  }
  %%%%=======trang trí tiêu đề chương
%%%CÁC KHAI BÁO MẶC ĐỊNH
  \setlength  \labelsep  {.5em}
  \setlength  \labelwidth{\leftmargini}
  \addtolength\labelwidth{-\labelsep}
  \@beginparpenalty -\@lowpenalty
  \@endparpenalty   -\@lowpenalty
  \@itempenalty     -\@lowpenalty
  \renewcommand\theenumi{\@arabic\c@enumi}
  \renewcommand\theenumii{\@alph\c@enumii}
  \renewcommand\theenumiii{\@roman\c@enumiii}
  \renewcommand\theenumiv{\@Alph\c@enumiv}
  \newcommand\labelenumi{\theenumi.}
  \newcommand\labelenumii{(\theenumii)}
  \newcommand\labelenumiii{\theenumiii.}
  \newcommand\labelenumiv{\theenumiv.}
  \renewcommand\p@enumii{\theenumi}
  \renewcommand\p@enumiii{\theenumi(\theenumii)}
  \renewcommand\p@enumiv{\p@enumiii\theenumiii}
  \newcommand\labelitemi{\color{Mapcolor}\faCheckCircle}
  \newcommand\labelitemii{\normalfont\bfseries \textendash}
  \newcommand\labelitemiii{\textasteriskcentered}
  \newcommand\labelitemiv{\textperiodcentered}
  \newenvironment{description}
                {\list{}{\labelwidth\z@ \itemindent-\leftmargin
                          \let\makelabel\descriptionlabel}}
                {\endlist}
  \newcommand*\descriptionlabel[1]{\hspace\labelsep
                                  \normalfont\bfseries #1}
  \newenvironment{verse}
                {\let\\\@centercr
                  \list{}{\itemsep      \z@
                          \itemindent   -1.5em%
                          \listparindent\itemindent
                          \rightmargin  \leftmargin
                          \advance\leftmargin 1.5em}%
                  \item\relax}
                {\endlist}
  \newenvironment{quotation}
                {\list{}{\listparindent 1.5em%
                          \itemindent    \listparindent
                          \rightmargin   \leftmargin
                          \parsep        \z@ \@plus\p@}%
                  \item\relax}
                {\endlist}
  \newenvironment{quote}
                {\list{}{\rightmargin\leftmargin}%
                  \item\relax}
                {\endlist}
  \if@compatibility
  \newenvironment{titlepage}
      {%
        \cleardoublepage
        \if@twocolumn
          \@restonecoltrue\onecolumn
        \else
          \@restonecolfalse\newpage
        \fi
        \thispagestyle{empty}%
        \setcounter{page}\z@
      }%
      {\if@restonecol\twocolumn \else \newpage \fi
      }
  \else
  \newenvironment{titlepage}
      {%
        \cleardoublepage
        \if@twocolumn
          \@restonecoltrue\onecolumn
        \else
          \@restonecolfalse\newpage
        \fi
        \thispagestyle{empty}%
        \setcounter{page}\@ne
      }%
      {\if@restonecol\twocolumn \else \newpage \fi
      \if@twoside\else
          \setcounter{page}\@ne
      \fi
      }
  \fi
  \newcommand\appendix{\par
    \setcounter{chapter}{0}%
    \setcounter{section}{0}%
    \gdef\@chapapp{\appendixname}%
    \gdef\thechapter{\@Alph\c@chapter}}
  \setlength\arraycolsep{5\p@}
  \setlength\tabcolsep{6\p@}
  \setlength\arrayrulewidth{.4\p@}
  \setlength\doublerulesep{2\p@}
  \setlength\tabbingsep{\labelsep}
  \skip\@mpfootins = \skip\footins
  \setlength\fboxsep{3\p@}
  \setlength\fboxrule{.4\p@}
  \@addtoreset {equation}{chapter}
  \renewcommand\theequation
    {\ifnum \c@chapter>\z@ \thechapter.\fi \@arabic\c@equation}
  \newcounter{figure}[chapter]
  \renewcommand \thefigure
      {\ifnum \c@chapter>\z@ \thechapter.\fi \@arabic\c@figure}
  \def\fps@figure{tbp}
  \def\ftype@figure{1}
  \def\ext@figure{lof}
  \def\fnum@figure{\figurename\nobreakspace\thefigure}
  \newenvironment{figure}
                {\@float{figure}}
                {\end@float}
  \newenvironment{figure*}
                {\@dblfloat{figure}}
                {\end@dblfloat}
  \newcounter{table}[chapter]
  \renewcommand \thetable
      {\ifnum \c@chapter>\z@ \thechapter.\fi \@arabic\c@table}
  \def\fps@table{tbp}
  \def\ftype@table{2}
  \def\ext@table{lot}
  \def\fnum@table{\tablename\nobreakspace\thetable}
  \newenvironment{table}
                {\@float{table}}
                {\end@float}
  \newenvironment{table*}
                {\@dblfloat{table}}
                {\end@dblfloat}
  %\newlength\abovecaptionskip
  %\newlength\belowcaptionskip
  \setlength\abovecaptionskip{10\p@}
  \setlength\belowcaptionskip{0\p@}
  \long\def\@makecaption#1#2{%
    \vskip\abovecaptionskip
    \sbox\@tempboxa{#1: #2}%
    \ifdim \wd\@tempboxa >\hsize
      #1: #2\par
    \else
      \global \@minipagefalse
      \hb@xt@\hsize{\hfil\box\@tempboxa\hfil}%
    \fi
    \vskip\belowcaptionskip}
  \DeclareOldFontCommand{\rm}{\normalfont\rmfamily}{\mathrm}
  \DeclareOldFontCommand{\sf}{\normalfont\sffamily}{\mathsf}
  \DeclareOldFontCommand{\tt}{\normalfont\ttfamily}{\mathtt}
  \DeclareOldFontCommand{\bf}{\normalfont\bfseries}{\mathbf}
  \DeclareOldFontCommand{\it}{\normalfont\itshape}{\mathit}
  \DeclareOldFontCommand{\sl}{\normalfont\slshape}{\@nomath\sl}
  \DeclareOldFontCommand{\sc}{\normalfont\scshape}{\@nomath\sc}
  \DeclareRobustCommand*\cal{\@fontswitch\relax\mathcal}
  \DeclareRobustCommand*\mit{\@fontswitch\relax\mathnormal}
  \newcommand\@pnumwidth{1.55em}
  \newcommand\@tocrmarg{2.55em}
  \newcommand\@dotsep{4.5}
  \setcounter{tocdepth}{2}
  \newcommand\tableofcontents{%
      \if@twocolumn
        \@restonecoltrue\onecolumn
      \else
        \@restonecolfalse
      \fi
      \chapter*{\contentsname
          \@mkboth{%
            \MakeUppercase\contentsname}{\MakeUppercase\contentsname}}%
      \@starttoc{toc}%
      
      \thispagestyle{fancy}
      \if@restonecol\twocolumn\fi
      }
  \newcommand*\l@part[2]{%
    \ifnum \c@tocdepth >-2\relax
      \addpenalty{-\@highpenalty}%
      \addvspace{2.25em \@plus\p@}%
      \setlength\@tempdima{3em}%
      \begingroup
        \parindent \z@ \rightskip \@pnumwidth
        \parfillskip -\@pnumwidth
        {\leavevmode
        \large \bfseries #1\hfil
        \hb@xt@\@pnumwidth{\hss #2%
                            \kern-\p@\kern\p@}}\par
        \nobreak
          \global\@nobreaktrue
          \everypar{\global\@nobreakfalse\everypar{}}%
      \endgroup
    \fi}
  \newcommand*\l@chapter[2]{%
    \ifnum \c@tocdepth >\m@ne
      \addpenalty{-\@highpenalty}%
      \vskip 1.0em \@plus\p@
      \setlength\@tempdima{1.5em}%
      \begingroup
        \parindent \z@ \rightskip \@pnumwidth
        \parfillskip -\@pnumwidth
        \leavevmode \bfseries
        \advance\leftskip\@tempdima
        \hskip -\leftskip
        #1\nobreak\hfil
        \nobreak\hb@xt@\@pnumwidth{\hss #2%
                                  \kern-\p@\kern\p@}\par
        \penalty\@highpenalty
      \endgroup
    \fi}
  \newcommand*\l@section{\@dottedtocline{1}{1.5em}{2.3em}}
  \newcommand*\l@subsection{\@dottedtocline{2}{3.8em}{3.2em}}
  \newcommand*\l@subsubsection{\@dottedtocline{3}{7.0em}{4.1em}}
  \newcommand*\l@paragraph{\@dottedtocline{4}{10em}{5em}}
  \newcommand*\l@subparagraph{\@dottedtocline{5}{12em}{6em}}
  \newcommand\listoffigures{%
      \if@twocolumn
        \@restonecoltrue\onecolumn
      \else
        \@restonecolfalse
      \fi
      \chapter*{\listfigurename}%
        \@mkboth{\MakeUppercase\listfigurename}%
                {\MakeUppercase\listfigurename}%
      \@starttoc{lof}%
      \if@restonecol\twocolumn\fi
      }
  \newcommand*\l@figure{\@dottedtocline{1}{1.5em}{2.3em}}
  \newcommand\listoftables{%
      \if@twocolumn
        \@restonecoltrue\onecolumn
      \else
        \@restonecolfalse
      \fi
      \chapter*{\listtablename}%
        \@mkboth{%
            \MakeUppercase\listtablename}%
          {\MakeUppercase\listtablename}%
      \@starttoc{lot}%
      \if@restonecol\twocolumn\fi
      }
  \let\l@table\l@figure
  \newdimen\bibindent
  \setlength\bibindent{1.5em}
  \newenvironment{thebibliography}[1]
      {\chapter*{\bibname}%
        \@mkboth{\MakeUppercase\bibname}{\MakeUppercase\bibname}%
        \list{\@biblabel{\@arabic\c@enumiv}}%
            {\settowidth\labelwidth{\@biblabel{#1}}%
              \leftmargin\labelwidth
              \advance\leftmargin\labelsep
              \@openbib@code
              \usecounter{enumiv}%
              \let\p@enumiv\@empty
              \renewcommand\theenumiv{\@arabic\c@enumiv}}%
        \sloppy
        \clubpenalty4000
        \@clubpenalty \clubpenalty
        \widowpenalty4000%
        \sfcode`\.\@m}
      {\def\@noitemerr
        {\@latex@warning{Empty `thebibliography' environment}}%
        \endlist}
  \newcommand\newblock{\hskip .11em\@plus.33em\@minus.07em}
  \let\@openbib@code\@empty
  \newenvironment{theindex}
                {\if@twocolumn
                    \@restonecolfalse
                  \else
                    \@restonecoltrue
                  \fi
                  \twocolumn[\@makeschapterhead{\indexname}]%
                  \@mkboth{\MakeUppercase\indexname}%
                          {\MakeUppercase\indexname}%
                  \thispagestyle{plain}\parindent\z@
                  \parskip\z@ \@plus .3\p@\relax
                  \columnseprule \z@
                  \columnsep 35\p@
                  \let\item\@idxitem}
                {\if@restonecol\onecolumn\else\clearpage\fi}
  \newcommand\@idxitem{\par\hangindent 40\p@}
  \newcommand\subitem{\@idxitem \hspace*{20\p@}}
  \newcommand\subsubitem{\@idxitem \hspace*{30\p@}}
  \newcommand\indexspace{\par \vskip 10\p@ \@plus5\p@ \@minus3\p@\relax}
  \renewcommand\footnoterule{%
    \kern-3\p@
    \hrule\@width.4\columnwidth
    \kern2.6\p@}
  \@addtoreset{footnote}{chapter}
  \newcommand\@makefntext[1]{%
      \parindent 1em%
      \noindent
      \hb@xt@1.8em{\hss\@makefnmark}#1}
  \newcommand\contentsname{Contents}
  \newcommand\listfigurename{List of Figures}
  \newcommand\listtablename{List of Tables}
  \newcommand\bibname{Bibliography}
  \newcommand\indexname{Index}
  \newcommand\figurename{Figure}
  \newcommand\tablename{Table}
  \newcommand\partname{Part}
  \newcommand\chaptername{Chapter}
  \newcommand\appendixname{Appendix}
  \def\today{\ifcase\month\or
    January\or February\or March\or April\or May\or June\or
    July\or August\or September\or October\or November\or December\fi
    \space\number\day, \number\year}
  \setlength\columnsep{10\p@}
  \setlength\columnseprule{0\p@}
  \pagestyle{headings}
  \pagenumbering{arabic}
  \if@twoside
  \else
    \raggedbottom
  \fi
  \if@twocolumn
    \twocolumn
    \sloppy
    \flushbottom
  \else
    \onecolumn
  \fi
%%%CÁC MẪU KHUNG=================================================================
  \newcounter{dang}\setcounter{dang}{0}
  \renewcommand{\thedang}{\arabic{dang}}
%%%TIÊU ĐỀ CHƯƠNG================================================================
  \RequirePackage{afterpage}
  \RequirePackage[explicit]{titlesec} % để gọi #1
  %--------------Tròn
  \newcommand{\tron}[1]{
    \begin{tikzpicture}[baseline=(A.base)]%
      \node[circle,draw=blue,line width=0.5pt,fill=white,inner sep=2pt,outer sep=1pt] (A){\color{white} #1};
      \node[circle,draw=none,fill=blue,inner sep=1pt,outer sep=1pt] (A){\color{white} #1};
    \end{tikzpicture}%
  }
  \RequirePackage{transparent}
  \titlespacing{\part}{0cm}{1.25cm}{2.5cm}[0cm] %1: , 2: Trên, 3: dưới
  \titleformat{\part}
  {\fontsize{25pt}{30pt}\fontfamily{qag}\selectfont\bfseries\color{white}} %định dạng chung
  {} %đánh số
  {-0.4cm}
  {
    \parbox{15cm}{\raggedleft\MakeUppercase{#1}} %neo vitri
    %\usebox{\Randomsymbol}
    \begin{tikzpicture}[remember picture,overlay,line join=round,line cap=round] 
      \fill[top color=Mapcolor!50,bottom color=white] (current page.north west) rectangle (current page.10);      
      \fill[Mapcolor!70] (current page.north west) --++(-90:8.5cm)--++(10:14cm)coordinate(A) arc (280:370:1.5cm)--++(95:6cm)--cycle;  
      \fill[Mapcolor] (current page.north west) --++(-90:7cm)--(A) arc (280:380:1.5cm)--++(110:6cm)--cycle;   
      \fill[Mapcolor!10] ($(current page.south west)+(0cm,0cm)$)--($(current page.south west)+(0cm,10.5cm)$) .. controls ($(current page.south west)+(4.24cm,1cm)$) and ($(current page.south west)+(7.26cm,1cm)$)  .. ($(current page.south west)+(11cm,8cm)$) 
      .. controls ($(current page.south west)+(15.5cm,17cm)$)  and ($(current page.south west)+(20.5cm,14cm)$) 
      ..  ($(current page.south east)+(0cm,14cm)$)--($(current page.south east)+(0cm,0cm)$)--cycle;
\fill[Mapcolor!60] ($(current page.south west)+(0cm,0cm)$)--($(current page.south west)+(0cm,8.5cm)$) .. controls ($(current page.south west)+(4.24cm,1.94cm)$) and ($(current page.south west)+(7.26cm,0.58cm)$)  .. ($(current page.south west)+(11cm,6cm)$) 
      .. controls ($(current page.south west)+(17cm,15cm)$)  and ($(current page.south west)+(20.02cm,11cm)$) 
      ..  ($(current page.south east)+(0cm,10cm)$)--($(current page.south east)+(0cm,0cm)$)--cycle;
\fill[Mapcolor] ($(current page.south west)+(0cm,0cm)$)--($(current page.south west)+(0cm,7cm)$) .. controls ($(current page.south west)+(4.24cm,1.94cm)$) and ($(current page.south west)+(8.02cm,0cm)$)  .. ($(current page.south west)+(12.32cm,5.5cm)$) 
      .. controls ($(current page.south west)+(15.46cm,9.5cm)$)  and ($(current page.south west)+(19cm,8.68cm)$) 
      ..  ($(current page.south east)+(0cm,6.5cm)$)--($(current page.south east)+(0cm,0cm)$)--cycle;
      \draw[Mapcolor!50,line width=2pt] ($(current page.south west)+(0cm,6cm)$) .. controls ($(current page.south west)+(4.24cm,1.94cm)$) and ($(current page.south west)+(8.02cm,0cm)$)  .. ($(current page.south west)+(12.32cm,5.5cm)$) 
      .. controls ($(current page.south west)+(15.46cm,9.5cm)$)  and ($(current page.south west)+(19cm,8cm)$) 
      ..  ($(current page.south east)+(0cm,5.5cm)$);
      \draw[Mapcolor!50,line width=2pt] ($(current page.south west)+(0cm,5cm)$) .. controls ($(current page.south west)+(4.24cm,1.94cm)$) and ($(current page.south west)+(8.02cm,0cm)$)  .. ($(current page.south west)+(12.32cm,5.5cm)$) 
      .. controls ($(current page.south west)+(15.46cm,9.5cm)$)  and ($(current page.south west)+(19cm,7cm)$) 
      ..  ($(current page.south east)+(0cm,4.5cm)$);
      \draw[Mapcolor!50,line width=2pt] ($(current page.south west)+(0cm,4cm)$) .. controls ($(current page.south west)+(4.24cm,1.94cm)$) and ($(current page.south west)+(8.02cm,0cm)$)  .. ($(current page.south west)+(12.32cm,5.5cm)$) 
      .. controls ($(current page.south west)+(15.46cm,9.5cm)$)  and ($(current page.south west)+(19cm,6cm)$) 
      ..  ($(current page.south east)+(0cm,3cm)$);
      \draw[Mapcolor!50,line width=2pt] ($(current page.south west)+(0cm,3cm)$) .. controls ($(current page.south west)+(4.24cm,1.94cm)$) and ($(current page.south west)+(8.02cm,0cm)$)  .. ($(current page.south west)+(12.32cm,5.5cm)$) 
      .. controls ($(current page.south west)+(15.46cm,9.5cm)$)  and ($(current page.south west)+(19cm,5cm)$) 
      ..  ($(current page.south east)+(0cm,1.5cm)$);
      \draw[Mapcolor!50,line width=2pt] ($(current page.south west)+(0cm,2cm)$) .. controls ($(current page.south west)+(4.24cm,1.94cm)$) and ($(current page.south west)+(8.02cm,0cm)$)  .. ($(current page.south west)+(12.32cm,5.5cm)$) 
      .. controls ($(current page.south west)+(15.46cm,9.5cm)$)  and ($(current page.south west)+(19cm,4cm)$) 
      ..  ($(current page.south east)+(0cm,0cm)$);
      \draw[line width=6pt,->,>=stealth',Mapcolor!30]($(current page.south west)+(5cm,2cm)$)--++(90:13cm);
      \draw[line width=6pt,->,>=stealth',Mapcolor!50]($(current page.south west)+(0cm,4cm)$)--++(0:17cm);
      \node[white] at ($(current page.south east)+(-3cm,2.5cm)$){\fontsize{35pt}{30pt}\fontfamily{qag}\selectfont 2025};
      %\fill[blue] ($(current page.south west)+(0cm,1.3cm)$)--($(current page.south west)+(0cm,5cm)$) .. controls (4.24cm,1.94cm) and (8.02cm,0cm) .. (12.32cm,5.12cm) .. controls (15.46cm,9.22cm) and (18.96cm,8.68cm) ..  ($(current page.south east)+(0cm,1.4cm)$)--cycle;
      %\node[scale=1,text=Mapcolor,inner sep=0pt,below] at ([xshift=-1cm,yshift=7cm]current page.center){\usebox{\Randomsymbol}}; 
      \draw[white,line width=2pt,rounded corners=2pt] ($(current page.north west)+(\letrai,-1.5cm)$) rectangle ($(current page.north west)+(\letrai+0.9cm,-0.7cm)$);
      \draw[white,line width=2pt] ($(current page.north west)+(\letrai+0.15cm,-1.4cm)$)--++(90:0.5cm)--++(-45:0.35cm)--++(45:0.35cm)--++(-90:0.35cm)coordinate (A) ;
      \draw[white,line width=2pt] ($(A)-(0.05,0.1)$)--++(0:0.2cm);   
      \node[white,right](a) at ($(current page.north west)+(\letrai+1.1cm,-1.1cm)$){\bf\Large\fontfamily{qag}\selectfont MAPSTUDY};
      \draw[white,line width=2pt,] ($(a.east)+(0.1cm,-0.3cm)$)--++(90:0.6);
      \node[right,white] (b) at ($(a.east)+(0.2cm,-0.05cm)$){\bf\fontsize{10}{11}\fontfamily{qag}\selectfont mapstudy.edu.vn};          
      %------------------------------------
      \checkoddpage\ifoddpage %nếu trang lẻ
      \def\sokc{\lephai}
      \else
      \def\sokc{\letrai}
      \fi
      %==============
      \node[scale=2,text=white,anchor=north east,inner sep=0pt]  (tam) at ([xshift=10.2*\sokc,yshift=-2.5*1.25cm]current page.north west){
        {\thepart}
      };
      %--	chương
      \node[left,xshift=-0.5cm,yshift=-3pt] (chuong) at (tam){\partname};
      %-- nội dung chương
      \node[scale=1,text=Mapcolor,inner sep=0pt] at ([xshift=2.5cm,yshift=3cm]current page.center){
       \parbox{16.5cm}{\MakeUppercase{\fontsize{36pt}{36pt}\selectfont #1}}
      };          
    \end{tikzpicture}    
    \thispagestyle{fancy}
    \setcounter{bangdethi}{0}
  }
  %================= Đn chương
  \titlespacing{\chapter}{0cm}{1.25cm}{2.5cm}[0cm] %1: , 2: Trên, 3: dưới
  \titleformat{\chapter}
  {\fontsize{25pt}{30pt}\fontfamily{qag}\selectfont\bfseries\color{white}} %định dạng chung
  {} %đánh số
  {-0.4cm}
  {
    \parbox{15cm}{\raggedleft\MakeUppercase{#1}} %neo vitri
    \begin{tikzpicture}[remember picture,overlay,line join=round,line cap=round] 
      \fill[top color=Mapcolor!50,bottom color=white] (current page.north west) rectangle (current page.10);
      \fill[Mapcolor!70] (current page.north west) --++(-90:8.5cm)--++(10:14cm)coordinate(A) arc (280:370:1.5cm)--++(95:6cm)--cycle;  
      \fill[Mapcolor] (current page.north west) --++(-90:7cm)--(A) arc (280:380:1.5cm)--++(110:6cm)--cycle;            
      %------------------------------------
      \checkoddpage\ifoddpage %nếu trang lẻ
      \def\sokc{\lephai}
      \else
      \def\sokc{\letrai}
      \fi
      %==============
      \node[scale=2,text=white,anchor=north east,inner sep=0pt]  (tam) at ([xshift=7.2*\sokc,yshift=-0.85*1.25cm]current page.north west){
        {\thechapter}
      };
      %--	chương
      \node[left,xshift=-12pt,yshift=-6pt] (chuong) at (tam){\chaptername};
      %-- nội dung chương
      \node[scale=1,text=white,right,inner sep=0pt] at ([xshift=0cm, yshift=-2.2cm]chuong.west){
       \parbox{12cm}{\raggedleft\MakeUppercase{#1}}
      };  
    \end{tikzpicture}
    \setcounter{bangdethi}{0}
  }
  [\vspace{1cm}]
  %\makeatletter
  \titleformat{name=\chapter,numberless}[display]
    {\fontsize{26pt}{18pt}\fontfamily{qag}\selectfont\bfseries\color{blue}} %định dạng chung
    {}
    {-1.5em}
    {%
    \begin{tikzpicture}[remember picture,overlay,line join=round,line cap=round] 
      \fill[top color=Mapcolor!50,bottom color=white] (current page.north west) rectangle (current page.10);
      \fill[Mapcolor!70] (current page.north west) --++(-90:8cm)--++(10:14cm)coordinate(A) arc (280:370:1.5cm)--++(95:6cm)--cycle;  
      \fill[Mapcolor] (current page.north west) --++(-90:6.5cm)--(A) arc (280:380:1.5cm)--++(110:6cm)--cycle;        
      \draw[white,line width=2pt,rounded corners=2pt] ($(current page.north west)+(\letrai,-1.5cm)$) rectangle ($(current page.north west)+(\letrai+0.9cm,-0.7cm)$);
      \draw[white,line width=2pt] ($(current page.north west)+(\letrai+0.15cm,-1.4cm)$)--++(90:0.5cm)--++(-45:0.35cm)--++(45:0.35cm)--++(-90:0.35cm)coordinate (A) ;
      \draw[white,line width=2pt] ($(A)-(0.05,0.1)$)--++(0:0.2cm);   
      \node[white,right](a) at ($(current page.north west)+(\letrai+1.1cm,-1.1cm)$){\bf\large\fontfamily{qag}\selectfont 0905.073.713};
      \draw[white,line width=2pt,] ($(a.east)+(0.1cm,-0.3cm)$)--++(90:0.6);
      \node[right,white] (b) at ($(a.east)+(0.2cm,-0.05cm)$){\bf\fontsize{10}{11}\fontfamily{qag}\selectfont GV: LÊ KHÁNH LOAN};      
      %------------------------------------
      \checkoddpage\ifoddpage %nếu trang lẻ
      \def\sokc{\lephai}
      \else
      \def\sokc{\letrai}
      \fi
      %==============
      \node[scale=2,text=white,anchor=north east,inner sep=0pt]  (tam) at ([xshift=7*\sokc,yshift=-0.85*1.25cm]current page.north west){
        {}
      };
      %--	chương
      \node[left,xshift=-6pt,yshift=-6pt] (chuong) at (tam){};
      %-- nội dung chương
      \node[scale=1,text=white,right,inner sep=0pt] at ([xshift=-2*\letrai, yshift=-2.8cm]current page.north west){
       \parbox{15cm}{\raggedleft\MakeUppercase{#1}}
      };  
    \end{tikzpicture}
      \setcounter{bangdethi}{0}
    }
    [
    \vspace{1.25cm}
    \thispagestyle{fancy}
    ]
  \newlength{\kcsection}
  \setlength{\kcsection}{-2pt}
  \titleformat{\section}
  {\color{white}\fontfamily{qag}\fontsize{18pt}{22pt}\selectfont\bfseries}
  {}  % label
  {0pt}  % sep
  {%
    \vspace{-\baselineskip}\begin{tcolorbox}[
      breakable,
      enhanced jigsaw,
      colback = Mapcolor,
      colbacktitle = Mapcolor,
      enlarge left by = 1.9cm,
      width = \dimexpr\linewidth - 2cm\relax,
      underlay unbroken and first = {
        \filldraw[Mapcolor,draw=white, line width=3pt, rounded corners=0.2cm, yshift=-1cm]
          ($(frame.north west)-(2,0.4)$) coordinate (bai1)
          -- ++(45:0.6) -- ++(0:2.4) coordinate (sbai)
          -- ++(-45:0.6) coordinate (sbaia)
          -- ($(sbai)-(1.2,2.1)$) coordinate (sbaib)
          -- cycle;
        \node[white,right=12pt] at (bai1){\large Bài};
        \node[text=white] at ($(sbai)+(-0.9,-0.75)$)
          {\fontsize{24pt}{1pt}\selectfont\thesection};
      },
      left=1cm,
      height=2cm,
      halign=center,
      valign=center,
      frame hidden,
      nobeforeafter,
      fontupper=\Large\bfseries\fontsize{18pt}{22pt}\fontfamily{qag}\selectfont\color{white},
    ]
    #1
    \end{tcolorbox}
    \setcounter{dang}{0}
    \setcounter{vd}{0}
    \setcounter{vidu}{0}
    \setcounter{dn}{0}
    \setcounter{dl}{0}
    \normalfont
  }

  \titleformat{\subsection}
    {\fontfamily{qag}\selectfont\bfseries\color{Mapcolor}}
    {\hspace*{-2.5mm}
      \begin{tikzpicture}[baseline=(SonA.base),line join=round,line cap=round]%
        \node[diamond,draw=none,ball color=Mapcolor,inner sep=2pt,outer sep=1pt] (SonA) {\color{white} {\thesubsection}};
        \draw[Mapcolor,line width=1pt] ([xshift=1pt]SonA.east)--++(135:0.35) ([xshift=1pt]SonA.east)--++(-135:0.35);
        \draw[Mapcolor,line width=1pt] ([xshift=-1pt]SonA.west)--++(45:0.35) ([xshift=-1pt]SonA.west)--++(-45:0.35);
        \draw[Mapcolor,line width=1pt] ([xshift=3.5pt]SonA.east)--++(135:0.2) ([xshift=3pt]SonA.east)--++(-135:0.2);
        \draw[Mapcolor,line width=1pt] ([xshift=-3.5pt]SonA.west)--++(45:0.2) ([xshift=-3pt]SonA.west)--++(-45:0.2);
      \end{tikzpicture}%
    }
    {0.45em}
    {#1\setcounter{subsubsection}{0}\normalfont}
    []
  \titleformat{\subsubsection}
    {\fontfamily{qag}\selectfont\bfseries\color{Mapcolor}}
    {
      \hspace*{-2mm}
      \begin{tikzpicture}[baseline=(char.base),line join=round,line cap=round]
        \node[text=white, fill=Mapcolor!85,rounded corners=3pt, inner sep=5pt, outer sep=2pt] (char) at (0,0){\thesubsubsection};
      \end{tikzpicture}%
    }
    {0.75em}
    {#1\normalfont\relax}
    []
  \newcommand\paratron[1]{\tikz[baseline=(char.base)]{
        %\draw (char) circle(0.5cm);
        \node[shape=circle,draw,double,double distance=0.5mm,inner sep=1pt,fill=Mapcolor,text = white,minimum width = 0.8cm] (char) {#1};}}
  \titleformat{\paragraph}
    {\bfseries\color{Mapcolor}}
    {\paratron{\theparagraph}}
    {0.75em}
    {#1}
    []
%%%ĐIỀU CHỈNH MỤC LỤC
  \RequirePackage{titletoc} 
  \titlecontents{part}[0pt]
    {\protect\addvspace{15pt}\color{Mapcolor}}
    {\contentslabel[\partname~\thecontentslabel.]{0pt}} % Thêm dấu chấm vào đây
    {\large\bfseries\fontfamily{qag}\selectfont \faBookmark~Phần } % Chỉ để biểu tượng
    {$\;$\titlerule$\;$\large\bfseries\thecontentspage}[]

  \titlecontents{chapter}[7pc] % nd cách trái
    {\addvspace{0pt}%
      \color{coolblack}\fontsize{13pt}{16pt}\fontfamily{put}\selectfont\bfseries
    }%
    {\contentslabel[\chaptertitlename\,\huge{\thecontentslabel}\normalsize.]{6.5pc}} %nhãn
    {\fontfamily{qag}\selectfont}
    {\hrulefill\bfseries\thecontentspage
    }%
  \titlecontents{section}[7.2pc]
    {\addvspace{0pt}\bfseries\color{Mapcolor}}
    {\fontsize{12pt}{15pt}\selectfont\sffamily\contentslabel[{Bài\,\thecontentslabel:}]{3.5pc}}
    {}
    {{\tiny\dotfill}
      \thecontentspage
    }
    []
  \newcommand*\tronsub[1]{\raisebox{0.1cm}{\tikz[baseline=(char.base)]{
      \draw[fill = Mapcolor,double, double distance = 1pt,line width = 1pt,draw=Mapcolor] (0,0.3) coordinate(char) circle (0.3cm);
      \node[white] at (0,0.3) {\bf\small #1};}}}
  \titlecontents{subsection}[7.6pc]
    {\addvspace{0pt}\color{Mapcolor}}
    {\fontsize{12pt}{15pt}\selectfont\sffamily\contentslabel[{\thecontentslabel.}]{2pc}}
    {}
    {{\tiny\dotfill}\thecontentspage}
    []
%%%CANH LÊ TRANG=================================================================
  \RequirePackage{xkeyval}
  \newlength{\letren}
  \newlength{\leduoi}
  \newlength{\letrai}
  \newlength{\lephai}  
  \setlength{\letren}{1cm} 
  \setlength{\leduoi}{0.8cm} 
  \setlength{\letrai}{2.2cm} 
  \setlength{\lephai}{1cm} 
  \DeclareRobustCommand{\subject}{VẬT LÍ 10}
  \DeclareRobustCommand{\headtitle}{PHONG TỎA VẬT LÍ - Tập 2}
  \DeclareRobustCommand{\foottitle}{ĐỊNH VỊ TRI THỨC - DẪN LỐI TƯ DUY}
  \DeclareRobustCommand{\Giaovien}{LÊ KHÁNH LOAN}
  \DeclareRobustCommand{\Phonenumber}{0905.073.713}
  \DeclareRobustCommand{\Tieudetrai}{LỚP TOÁN CÔ LOAN}
  \DeclareRobustCommand{\dongcham}[1]{\dotlineEX{#1}}
  \RequirePackage[a4paper, left=\letrai,right=\lephai,top=\letren,bottom=\leduoi, footskip=24pt,headsep=6pt, includeheadfoot]{geometry}
    \renewcommand\normalsize{\@setfontsize\normalsize{12.5}{18.2}}
    \newcommand{\Numone}{\fontsize{50pt}{50pt}\selectfont}
    \newcommand{\Linefont}{\fontsize{8pt}{26.5pt}\selectfont}
    \DeclareRobustCommand{\titlefont}{% 
    \bfseries\fontfamily{phv}\fontseries{b}\myfont{15.5pt}{24pt}}
  \RequirePackage[cal=boondoxo]{mathalfa}
  \RequirePackage{pifont}
  \RequirePackage{fontawesome}
%%%%HEADER - FOOTER======================================
  
\newcommand{\myfancyhead}{
  \begin{tikzpicture}[remember picture,overlay]
    \checkoddpage
    \ifoddpage	
    \filldraw[Mapcolor] (current page.north east)to ($(current page.north east)-(0,1.5cm)$)to[rounded corners = 5pt] ($(current page.north east)-(10cm,1.5cm)$)to[rounded corners = 5pt] ($(current page.north east)-(11cm,0.7cm)$) to ($(current page.north west)-(0cm,0.7cm)$) to (current page.north west);
    %\draw[Mapcolor,line width = 2pt,dashed] ($(current page.north east)-(0,1.7cm)$) to[rounded corners = 5pt] ($(current page.north east)-(10.1cm,1.7cm)$)to[rounded corners = 5pt] ($(current page.north east)-(11.1cm,0.9cm)$) to ($(current page.north west)-(0cm,0.9cm)$);
	\filldraw[Mapcolor,opacity=0.2] ($(current page.north east)-(10.3cm,1.5cm)$) to[rounded corners = 5pt] ($(current page.north east)-(11.05cm,0.85cm)$) to ($(current page.north west)-(0cm,0.85cm)$) to (current page.north west) to ($(current page.north west)-(0,1.5cm)$); \node[left,white] (a) at ($(current page.north east)-(\lephai,1cm)$){\bf\fontsize{11}{11}\fontfamily{qag}\selectfont \subject};
      \draw[white,line width=2pt,] ($(a)-(1.1cm,0.3cm)$)--++(90:0.6);
      \node[left,white] (b) at ($(a)-(1.3cm,0cm)$){\bf\fontsize{11}{11}\fontfamily{qag}\selectfont \headtitle};
    \else
      \filldraw[Mapcolor] (current page.north west) to ($(current page.north west)-(0,1.5cm)$) to[rounded corners = 5pt] ($(current page.north west)+(10cm,-1.5cm)$)to[rounded corners = 5pt] ($(current page.north west)+(11cm,-0.7cm)$)to ($(current page.north east)+(0cm,-0.7cm)$) to (current page.north east);
      %\draw[Mapcolor,line width = 2pt,dashed] ($(current page.north west)-(0,1.7cm)$) to ($(current page.north west)+(10.1cm,-1.7cm)$)to[rounded corners = 5pt] ($(current page.north west)+(11.1cm,-0.9cm)$)to ($(current page.north east)+(0cm,-0.9cm)$);
      \filldraw[Mapcolor,opacity=0.2] ($(current page.north west)+(10.3cm,-1.5cm)$) to[rounded corners = 5pt] ($(current page.north west)+(11cm,-0.85cm)$) to ($(current page.north east)-(0cm,0.85cm)$) to (current page.north east) to ($(current page.north east)-(0,1.5cm)$);
      %\draw[white,line width=2pt,rounded corners=2pt] ($(current page.north west)+(\lephai,-1.4cm)$) rectangle ($(current page.north west)+(\lephai+0.9cm,-0.6cm)$);
      %\path ($(current page.north west)+(\lephai+0.15cm,-1.32cm)$)++(90:0.5cm)++(-45:0.35cm)++(45:0.35cm)++(-90:0.35cm)coordinate (A) ;
      %\draw[white,line width=2pt] ($(A)-(0.05,0.1)$)--++(0:0.2cm);   
      \node[white,right](a) at ($(current page.north west)+(\lephai,-1cm)$){\bf\Large\fontfamily{qag}\selectfont\faPhone~\Phonenumber};
      \draw[white,line width=2pt,] ($(a.east)+(0.1cm,-0.3cm)$)--++(90:0.6);
      \node[right,white] (b) at ($(a.east)+(0.2cm,-0.05cm)$){\bf\fontsize{10}{11}\fontfamily{qag}\selectfont GV: \Giaovien};
      \node[left,Mapcolor] (b) at ($(current page.north east)-(\letrai,1.15cm)$){\bf\fontsize{10}{11}\fontfamily{qag}\selectfont Biên soạn theo cấu trúc mới 2026};
    \fi
  \end{tikzpicture}
  }
  \newcommand{\myfancyfoot}{
    \begin{tikzpicture}[remember picture,overlay]		
    \checkoddpage		
    \ifoddpage
    \filldraw[Mapcolor,opacity=0.2] (current page.south east)to ($(current page.south east)+(0,1.6cm)$) to[rounded corners = 5pt] ($(current page.south east)+(-9cm,1.6cm)$)to[rounded corners = 5pt] ($(current page.south east)+(-10cm,0.7cm)$)to ($(current page.south west)+(0cm,0.7cm)$)to (current page.south west);
    %\draw[Mapcolor,line width = 2pt,dashed] ($(current page.south east)+(0,1.8cm)$) to[rounded corners = 5pt] ($(current page.south east)+(-9.1cm,1.9cm)$)to[rounded corners = 5pt] ($(current page.south east)+(-10.1cm,0.9cm)$)to ($(current page.south west)+(0cm,0.9cm)$);
    \filldraw[Mapcolor] ($(current page.south east)+(-9.4cm,1.5cm)$)to[rounded corners = 5pt] ($(current page.south east)+(-10.15cm,0.85cm)$)to ($(current page.south west)+(0cm,0.85cm)$)to (current page.south west)to ($(current page.south west)+(0,1.5cm)$);
    \node[right, white] at ($(current page.south west)+(\letrai,1.2cm)$){\bf\fontfamily{qag}\selectfont\myfont{11}{32} \foottitle};
      \node[left,Mapcolor] at ($(current page.south east)-(\lephai,-1.1)$){\bf\fontsize{10}{11}\fontfamily{qag}\selectfont\faHandORight~Trang \fontsize{16}{11}\selectfont\thepage};
    \else
    \filldraw[Mapcolor,opacity=0.2] (current page.south west)to ($(current page.south west)+(0,1.6cm)$) to[rounded corners = 5pt] ($(current page.south west)+(9cm,1.6cm)$)to[rounded corners = 5pt] ($(current page.south west)+(10cm,0.7cm)$)to ($(current page.south east)+(0cm,0.7cm)$)to (current page.south east);
    %\draw[Mapcolor,line width = 2pt,dashed] ($(current page.south west)+(0,1.7cm)$) to[rounded corners = 5pt] ($(current page.south west)+(9.1cm,1.7cm)$)to[rounded corners = 5pt] ($(current page.south west)+(10.1cm,0.9cm)$)to ($(current page.south east)+(0cm,0.9cm)$);
    \filldraw[Mapcolor] ($(current page.south west)+(9.3cm,1.6cm)$)to[rounded corners = 5pt] ($(current page.south west)+(10.15cm,0.85cm)$)to ($(current page.south east)+(0.cm,0.85cm)$)to (current page.south east)to ($(current page.south east)+(0,1.6cm)$);
    \node[left, white] at ($(current page.south east)-(\letrai,-1.2)$){\bf\fontfamily{qag}\selectfont\myfont{11}{32} \foottitle};
      \node[right,Mapcolor] at ($(current page.south west)+(\lephai,1.1)$){\bf\fontsize{10}{11}\fontfamily{qag}\selectfont\faHandORight~Trang \fontsize{16}{11}\selectfont\thepage};     
    \fi
    %\draw[Mapcolor,line width=2pt] ($(current page.south west)+(0,1.2)$)--($(current page.south east)+(0,1.2)$);
    \end{tikzpicture}
  }
  \newcommand{\headmucluc}{%
    \boldmath
    \begin{tikzpicture}[remember picture,overlay,>=stealth]
      \checkoddpage\ifoddpage %nếu trang lẻ
      \def\kctrai{\phai} \def\kcphai{\trai}
      \pgfmathsetmacro{\letren}{-\letren+0.5*\topset} 
      \path ([xshift=\kctrai cm, yshift=-1 cm]current page.north west) coordinate (AA)
      ++(\textwidth,0)coordinate (BB); 
      %---- bên phải
      \foreach \ii/\mau in {1.35/Mapcolor!30,1/Mapcolor!50,0.5/Mapcolor!70,-0.1/Mapcolor}{
        \fill[\mau,draw=white,line width=2pt] ([xshift=1pt,yshift=1pt]BB)--++(-0.75 cm-\ii cm-3pt,0)
        --++(-0.3,0.5cm+1pt)--++(0.75 cm +\ii cm+3pt+0.3 cm,0)--cycle
        ;
      }
      \node[anchor=south east,text=Mapcolor,inner sep=0pt] at ([xshift=-0.75 cm-1.35cm-10pt]BB){\fontfamily{qag}\fontsize{10pt}{12pt}\selectfont\nouppercase{\trenchanphai}};
      %-----đường kẻ
      \draw[\maukefoot, line width=1.5pt] (AA) --(BB);
      \node[text=Mapcolor, anchor=west,inner sep=0pt] (trang) at ([yshift=6pt]AA){\fontfamily{qag}\fontsize{10pt}{12pt}\selectfont Trang};
      \node[rectangle, fill=Mapcolor, text=white, anchor=west,inner sep=3pt, rounded corners=4pt] at ([xshift=3pt,yshift=1.5pt]trang.east){\fontfamily{qag}\fontsize{10pt}{18pt}\selectfont\bfseries\thepage};
      \else
      \def\kctrai{\trai} \def\kcphai{\phai}
      \pgfmathsetmacro{\kctren}{-\tren+0.5*\topset} 
      \path ([xshift=\kctrai cm, yshift=\kctren cm]current page.north west) coordinate (AA)
      ++(\textwidth,0)coordinate (BB); 
      %---- màu bên trái
      \foreach \ii/\mau in {1.35/Mapcolor!30,1/Mapcolor!50,0.5/Mapcolor!70,-0.1/Mapcolor}{
        \fill[\mau,draw=white,line width=2pt] ([xshift=-1pt,yshift=1pt]AA)--++(0.75 cm+\ii cm+3pt,0)--++(0.3,0.5cm+1pt)--++(-0.75 cm -\ii cm-3pt-0.3cm,0)--cycle;
      }
      %---- chữ
      \node[anchor=south west,text=Mapcolor,inner sep=0pt] at ([xshift=0.75 cm+1.35cm+10pt,yshift=1pt]AA){\fontfamily{qag}\fontsize{10pt}{12pt}\selectfont\nouppercase{\trenletrai}};
      %-----đường kẻ
      \draw[\maukefoot, line width=1.5pt] (AA) -- (BB);
      %-----bên phải
      \node[rectangle, fill=Mapcolor, text=white, anchor=south east,inner sep=3pt, rounded corners=4pt] (sotrang) at ([xshift=0cm,yshift=1.5pt]BB){\fontfamily{qag}\fontsize{10pt}{12pt}\selectfont\bfseries\thepage};
      \node[text=Mapcolor, anchor=east,inner sep=0pt] at ([xshift=-3pt,yshift=-1pt]sotrang.west){\fontfamily{qag}\fontsize{10pt}{18pt}\selectfont Trang};
      %-------------------
      \fi
    \end{tikzpicture}%
  }
  \RequirePackage{fancyhdr}
    \pagestyle{fancy}
    \fancyhf{}
    \fancyhead[LO,RE]{\myfancyhead}
    \fancyfoot[LO,RE]{\myfancyfoot}
    \def\subsectiontitle{}
    \renewcommand{\footrulewidth}{0pt}
    \renewcommand{\headrulewidth}{0pt}
    \newcounter{coter}
    \setcounter{coter}{0}
    \def\sodem{\stepcounter{coter}\thecoter}
%%%KHAI BÁO CHO GÓI EX_TEST=================================================
  \RequirePackage{colortbl}
  \RequirePackage{Packages/ex_test}  
  \OPTN{viettat=1}
  \newcommand{\OldSqsubset}{\usebox{\oldsqsymbol}}
  \renewcommand{\nameex}{\normalsize\color{Mapcolor}\faEdit~\fontfamily{qag}\selectfont Câu}
  \renewcommand{\namebt}{\normalsize\color{Mapcolor}\faEdit~\fontfamily{qag}\selectfont Bài}
  \renewcommand{\namevd}{\normalsize\color{Mapcolor}\faEdit~\fontfamily{qag}\selectfont Ví dụ}  
  \newcounter{Made}
  \setcounter{Made}{0}
  \renewcommand{\theMade}{\arabic{Made}}
  \newcommand{\TieudeT}[1]{\begingroup
    \begin{tcolorbox}[enhanced jigsaw,colframe=Mapcolor, colback = white!5,opacityback=0.5,sidebyside,righthand width=0.63\textwidth]
      \centerline{\color{Mapcolor} \faDrupal~\bf\fontfamily{qag}\selectfont \Tieudetrai}
      \vspace{0.5\baselineskip}
      \centerline{\bf\fontfamily{qag}\selectfont ĐỀ LUYỆN TẬP SỐ \circEX{\bf\stepcounter{Made}\theMade}}
      \tcblower
      \centerline{\bf\fontfamily{qag}\selectfont\color{Mapcolor} #1}
      \centerline{Môn: \bf TOÁN 10}
      \centerline{\it Thời gian làm bài: 90 phút (không tính thời gian giao đề)}
      \end{tcolorbox}
      %\addcontentsline{toc}{subsection}{\vspace{.15cm}\sffamily \faFolderOpen\ Dạng~\thedang. #1}
      \addcontentsline{toc}{subsection}{\hspace{-5.5pc}\faTags~\textbf{Đề số \theMade:}~\sffamily #1}
      \endgroup
  }
  
	\RequirePackage{enumitem}
  \setlist[enumerate,1]{itemsep=-2pt,topsep=-2pt,label=\protect\circEX{\sffamily\bfseries\arabic*}}
  \setlist[itemize]{itemsep=-2pt,topsep=-2pt}
  \newenvironment{lithuyet}{}{}
  \newenvironment{dethi}{}{}
  % Khai báo tùy chọn `tuychon` với giá trị mặc định là HS1
  \DeclareOptionX{tuychon}[HS1]{\def\MAPClass@tuychon{#1}}
  \DeclareOptionX{mausac}[01]{\def\MAPClass@mausac{#1}}
  \DeclareOptionX{pagesize}[BookA4]{\def\MAPClass@pagesize{#1}}
  \DeclareOptionX{company}[BookA4]{\def\MAPClass@company{#1}}
  \ProcessOptionsX*
  % Thực hiện hành động khi kết thúc việc nạp class
  \AtEndOfClass{
      \ifx\MAPClass@tuychon\undefined
          \def\MAPClass@tuychon{HS1} % Nếu không có giá trị, gán mặc định là HS1
      \fi      
      % Xử lý theo tùy chọn
      \ifthenelse{\equal{\MAPClass@tuychon}{HS1}}{
        \renewcommand{\FalseEX}{\stepcounter{dapan}\fontfamily{qag}\selectfont\textbf{\textcolor{Mapcolor}{\Alph{dapan}.}}\ignorespaces}
        \renewcommand{\beginboxI}{%
          \begin{tcolorbox}[%
            enhanced jigsaw, breakable,
            before skip=1pt, after skip=1pt,%
            left=0mm, right=0mm, top=0pt, bottom=0mm,
            boxrule=1mm,opacityback=0,%
            colframe=white, colback=white, boxrule=0pt]%
        }
        \thembox{vidu}
        %\dotlinefull{vidu}
        \thembox{vd}
        %\dotlinefull{vd}
        \createbox{ex}
        \createbox{bt}
        \hideenviron{bangDA}
        \def\loigiaiEX{}
      }{%
      \ifthenelse{\equal{\MAPClass@tuychon}{HS2}}{
		  \renewcommand{\beginboxI}{%
				\begin{tcolorbox}[%
					enhanced jigsaw, pad at break*=1mm,%
					before skip=2mm, after skip=3mm,%
					left=0mm, right=0mm, top=0pt, bottom=0mm,
					opacityback=0,%
					colframe=white, colback=white, boxrule=0pt]%
			}
        \createbox{vd}
        \createbox{ex}
        \createbox{bt}
        \dotlinefull{ex}
        \def\loigiaiEX{}
        \hideenviron{bangDA}
      }{%
      \ifthenelse{\equal{\MAPClass@tuychon}{HS3}}{
        \createbox{vidu}
        \createbox{ex}
        \createbox{bt}
        \dotlinefull{vd}
        \dotlinefull{ex}
        \dotlinefull{bt}
        \hideenviron{bangDA}
        \renewcommand{\beginboxI}{%
          \begin{tcolorbox}[%
            enhanced jigsaw, pad at break*=1mm,%
            before skip=2mm, after skip=3mm,%
            left=2mm, right=2mm, top=2pt, bottom=2mm,
            boxrule=1mm,opacityback=1,%
            lifted shadow={1mm}{-2mm}{3mm}{0.1mm}{black!50!white},
            colframe=Mapcolor, colback=Mapcolor!10, boxrule=2pt]%
        }
        \def\loigiaiEX{}
        \renewcommand{\dotlineEXb}[2][1]{%
          \baselineskip=\numexpr\linespacing*22\relax pt%
          \setlength{\parskip}{0pt}%
          \xdef\numlinedot{\the\numexpr#2*#1}%
          \ifnum#1=1%
            \par\foreach \dotline in {1,...,#2}
              {\noindent{\newline}\par}%
          \else%
            \xdef\mtcs{\the\multicolsep}%
            \xdef\csr{\the\columnseprule}%
            \setlength{\multicolsep}{1pt}%
            \setlength{\columnseprule}{\linebetween}%
            \def\columnseprulecolor{\color{\linebetweencolor}}%
            \begin{multicols}{#1}%
              \foreach \dotline in {1,...,\numlinedot}
                {\noindent{\fontsize{\dotsize}{0pt}\selectfont\color{\dotcolor}\hrulefill}\par}%
            \end{multicols}%
            \setlength{\columnseprule}{\csr}%
            \setlength{\multicolsep}{\mtcs}%
          \fi%
          \vspace{5pt}\noindent%
          \setlength{\parskip}{\pskip}%
          \baselineskip=\blskip%
        }
        \renewcommand{\myfancyhead}{
          \begin{tikzpicture}[remember picture,overlay]
            \draw[step=5mm,lightgray,opacity=0.5] (current page.south west) grid (current page.north east);				
          \end{tikzpicture}
        }
        \renewcommand{\myfancyfoot}{}
      }{%
      \ifthenelse{\equal{\MAPClass@tuychon}{GV1}}{
        \makeatletter
        \def\applykindSA{0} % Để huỷ áp dụng \kindSA
        \def\colorEX{\color{red}}
        \renewcommand{\TrueTF} {\stepcounter{dapan}\circEX{\bfseries\DapAnTF}\ignorespaces}
        \renewcommand{\TrueEX} {\stepcounter{dapan}\circEX{\bfseries\Alph{dapan}}\ignorespaces}
        \renewcommand{\TrueEXn}{\stepcounter{dapan}\squareEX{\bfseries\DapAnN}\ignorespaces}
        \renewcommand{\TrueUn}{\squareEX{\bfseries\Alph{dapan}}}
        \renewcommand{\True}{%
          \ifnum\cotDS=1\gdef\TrueX{\circEX{Đ}}\else\gdef\TrueX{\circEX{\footnotesize\tickT}}\fi\gdef\FalseX{}%
          \leavevmode\colorEX%
        }
        %\choiceTF
        \renewcommand{\writekeyTFone}{\ifnum\cotDS=1\gdef\TrueX{S}\else\gdef\TrueX{}\fi\gdef\FalseX{\circEX{\tickF}}}
        \renewcommand{\writekeyTF}{%
          \ifthenelse{\equal{\cotDS}{2}}
          {&\vspace{-1pt}\centering\leavevmode\TrueX%
            &\vspace{-3pt}\parbox[t]{\linewidth}{\centering\leavevmode\FalseX}%
              \gdef\TrueX{}\gdef\FalseX{\circEX{\footnotesize\tickF}}}
          {\ifthenelse{\equal{\cotDS}{1}}
          {& \parbox[t]{\linewidth}{\centering\leavevmode\TrueX}\gdef\TrueX{S}}
          {}}%
        }
        %\shortans
        \def\kindSA{ShowSAKeyColor}
        %=============
        %% match
        %=============
        \renewcommand{\ansmatch}{\viewansmatch{squareEX}{\;>>>>\;}{cotII}}
        \renewcommand{\exitshowansEX}[1]{
          \AtBeginEnvironment{#1}{
            \renewcommand{\ansmatch}{\viewansmatch{squareEX}{\;>>>>\;}{cotII}}
          }
        }
        \renewcommand{\exithideansEX}[1]{%
          \makeatletter
          \AtBeginEnvironment{#1}{%
            \def\kindSA{ShowSAKeyColor}%
            \def\colorEX{\color{Mapcolor}}%
            \renewcommand{\TrueTF}{\stepcounter{dapan}\squareEX{\bfseries\DapAnTF}\ignorespaces}
            \renewcommand{\TrueEX}{\stepcounter{dapan}\squareEX{\bfseries\Alph{dapan}}\ignorespaces}
            \renewcommand{\TrueEXn}{\stepcounter{dapan}\squareEX{\bfseries\DapAnN}\ignorespaces}
            \renewcommand{\TrueUn}{\squareEX{\bfseries\Alph{dapan}}}
            \renewcommand{\writekeyTFone}{\ifnum\cotDS=1\gdef\TrueX{S}\else\gdef\TrueX{}\fi\gdef\FalseX{\tickF}}
            \renewcommand{\writekeyTF}{%
              \ifthenelse{\equal{\cotDS}{2}}
              {&\vspace{-1pt}\centering\leavevmode\TrueX%
                &\vspace{-3pt}\parbox[t]{\linewidth}{\centering\leavevmode\FalseX}%
                  \gdef\TrueX{}\gdef\FalseX{\tickF}}
              {\ifthenelse{\equal{\cotDS}{1}}
              {& \parbox[t]{\linewidth}{\centering\leavevmode\TrueX}\gdef\TrueX{S}}
              {}}%
            }
            \renewcommand{\ansmatch}{\viewansmatch{squareEX}{\;>>>>\;}{cotII}}
          }
        }
      }{%
      \ifthenelse{\equal{\MAPClass@tuychon}{GV2}}{
        \makeatletter
        \def\applykindSA{0} % Để huỷ áp dụng \kindSA
        \def\colorEX{\color{Mapcolor}}
        \renewcommand{\TrueTF} {\stepcounter{dapan}\circEX{\bfseries\DapAnTF}\ignorespaces}
        \renewcommand{\TrueEX} {\stepcounter{dapan}\circEX{\bfseries\Alph{dapan}}\ignorespaces}
        \renewcommand{\TrueEXn}{\stepcounter{dapan}\circEX{\bfseries\DapAnN}\ignorespaces}
        \renewcommand{\TrueUn}{\squareEX{\bfseries\Alph{dapan}}}
        \renewcommand{\True}{%
          \ifnum\cotDS=1\gdef\TrueX{\circEX{Đ}}\else\gdef\TrueX{\circEX{\footnotesize\tickT}}\fi\gdef\FalseX{}%
          \leavevmode\colorEX%
        }
        \renewcommand{\TF}[1]{\EXboxTF{\textbf{\colorEX#1}}}
        \renewcommand{\boxEX}[2][9pt]{%
          \setbox1=\hbox{#2}%
          \def\sizeboxEX{#1}%
          \ \EXbox{#2}\ %
        }
        \renewcommand{\sh}[1]{\text{#1}}
        \showanswers
        %Xuất hiện chữ Lời giải trong môi trường onlysolution
        \makeatletter
        \renewcommand{\do@onlysolution}[1]{%
          \par\noindent\textbf{\loigiaiEX}%
          \ifshowanswers
          \@ifnextchar\immini
          {}
          { \@ifnextchar\begin
          {}
          {\par\noindent}
          }
          #1%
          \fi
          \ifthenelse{\equal{\keyEX}{}}{}
              {\leavevmode\unskip\textchoice\ignorespaces\xdef\keyEX{}}%
        }
        \renewcommand{\loigiai}[1]{%
          \labelex\gdef\labelex{}%
          \ifInTcolorbox\ifdef{\endbox}{\endbox\gdef\endbox{}}{}\fi%
          \begin{onlysolution}
            #1
          \end{onlysolution}
        }
        \renewcommand{\True}{%
          \ifnum\cotDS=1\gdef\TrueX{Đ}\else\gdef\TrueX{\circEX{\footnotesize\tickT}}\fi\gdef\FalseX{}%
        }
        %\choiceTF
        \renewcommand{\writekeyTFone}{\ifnum\cotDS=1\gdef\TrueX{S}\else\gdef\TrueX{}\fi\gdef\FalseX{\circEX{\footnotesize\tickF}}}
        \renewcommand{\writekeyTF}{%
          \ifthenelse{\equal{\cotDS}{2}}
          {&\vspace{-1pt}\centering\leavevmode\TrueX%
            &\vspace{-3pt}\parbox[t]{\linewidth}{\centering\leavevmode\FalseX}%
              \gdef\TrueX{}\gdef\FalseX{\circEX{\footnotesize\tickF}}}
          {\ifthenelse{\equal{\cotDS}{1}}
          {& \parbox[t]{\linewidth}{\centering\leavevmode\TrueX}\gdef\TrueX{S}}
          {}}%
        }
        %\shortans
        \def\kindSA{ShowSAKeyLG}
        %%=================
        %% match
        %%=================
        \renewcommand{\ansmatch}{\viewansmatch{fbox}{\;>>>>\;}{cotII}}
        \renewcommand{\exithideansEX}[1]{%
          \makeatletter
          \AtBeginEnvironment{#1}{%
            \def\kindSA{ShowSAKeyLG}%
            \renewcommand{\writekeyTFone}{\ifnum\cotDS=1\gdef\TrueX{S}\else\gdef\TrueX{}\fi\gdef\FalseX{\tickF}}
            \renewcommand{\writekeyTF}{%
              \ifthenelse{\equal{\cotDS}{2}}
              {&\vspace{-1pt}\centering\leavevmode\TrueX%
                &\vspace{-3pt}\parbox[t]{\linewidth}{\centering\leavevmode\FalseX}%
                  \gdef\TrueX{}\gdef\FalseX{\tickF}}
              {\ifthenelse{\equal{\cotDS}{1}}
              {& \parbox[t]{\linewidth}{\centering\leavevmode\TrueX}\gdef\TrueX{S}}
              {}}%
            }
            \renewcommand{\ansmatch}{\viewansmatch{fbox}{\;>>>>\;}{cotII}}
            \renewcommand{\loigiai}[1]{%
              \labelex\gdef\labelex{}%
              \ifInTcolorbox\ifdef{\endbox}{\endbox\gdef\endbox{}}{}\fi%
              \begin{onlysolution}
              ##1
              \end{onlysolution}
            }
          }
        }
      }{%
      \ifthenelse{\equal{\MAPClass@tuychon}{GV3}}{
        \makeatletter
        \createbox{vd}
        \createbox{ex}
        \createbox{bt}
        \showanswers
        \renewcommand{\loigiai}[1]{%
          \labelex\gdef\labelex{}%
          \writeansbook{#1}%
        }
        \AtBeginEnvironment{chc}{%
          \renewcommand{\loigiai}[1]{%
            \labelex\gdef\labelex{}%
            \ifnum\DGNL=0\ifdef{\labelthmex}{\let\labelthm\labelthmex}{\gdef\labelthm{\nameex}}\fi%
            \writeansbook{#1}%
          }
        }
        \renewcommand{\writeansbook}[1]{%
          \ifthenelse{\equal{\detokenize\expandafter{\addques}}{}}{}
            {\Writetofile{ansbook}{\detokenize\expandafter{\addques}}}%
          \ifthenelse{\equal{\detokenize\expandafter{\addans}}{}}{}
            {\Writetofile{ansbook}{\detokenize\expandafter{\addans}}}%
          \Writetofile{ansbook}{\string\def\string\namebook{\detokenize\expandafter{\labelthm}}\string\def\string\kinditch{\kinditch}^^J\string\begin{Solbook}{\Currentlabel}}%
          \ifthenelse{\equal{\saveFileAns}{}}{}{%
            \ifnum\chType=0
              \Writetofile{ansbook}{\detokenize\expandafter{\selectchoice}\ \string\circEX{\saveFileAns}}%
            \else\ifnum\chType=2
              \Writetofile{ansbook}{\detokenize\expandafter{\selectchoice}\ \string\fbox{\saveFileAns}}%
            \else\ifnum\chType=1
              \Writetofile{ansbook}{\detokenize\expandafter{\selectchoiceTF}: \string\Fullcirc\saveFileAns}%
            \else\ifnum\chType=-2
              \Writetofile{ansbook}{\detokenize\expandafter{\selectchoiceTF}: \saveFileAns}%
            \else\ifnum\chType=-1
              \Writetofile{ansbook}{\detokenize\expandafter{\selectshortans} \saveKQ.}%
            \fi\fi\fi\fi\fi%
          }%
          \Writetofile{ansbook}{\string\par\string\noindent^^J\detokenize{#1}^^J\string\end{Solbook}^^J}%
        }
      }{
      \ifthenelse{\equal{\MAPClass@tuychon}{GV4}}
        {
          \renewcommand{\beginboxI}{%
            \begin{tcolorbox}[%
              enhanced jigsaw, pad at break*=1mm,%
              before skip=2mm, after skip=3mm,%
              left=0mm, right=0mm, top=0pt, bottom=0mm,
              boxrule=1mm,opacityback=0,%
              colframe=white, colback=white, boxrule=0pt]%
          }
          \createbox{vidu}
          \createbox{ex}
          \createbox{bt}
          \hideenviron{dethi}
          \hideenviron{lithuyet}
        }{%
          \typeout{Không có tùy chọn hợp lệ}}}}}}}}
      \ifthenelse{\equal{\MAPClass@company}{BookA4}}{
        \setlength{\letren}{1cm} 
        \setlength{\leduoi}{1cm} 
        \setlength{\letrai}{2.2cm} 
        \setlength{\lephai}{1cm} 
        \newgeometry{a4paper, left=\letrai,right=\lephai,top=\letren,bottom=\leduoi, footskip=32pt,headsep=6pt, includeheadfoot}     
      }{
      \ifthenelse{\equal{\MAPClass@pagesize}{DethiA4}}{
          \setlength{\letren}{1.3cm} 
          \setlength{\leduoi}{1.3cm} 
          \setlength{\letrai}{0.5cm} 
          \setlength{\lephai}{0.5cm} 
          \newgeometry{a4paper, left=\letrai,right=\lephai,top=\letren,bottom=\leduoi, footskip=24pt,headsep=6pt, includeheadfoot} 
        }{
            \ifthenelse{\equal{\MAPClass@pagesize}{NXB}}{
              \renewcommand\normalsize{\@setfontsize\normalsize{12}{16}}
              \setlength{\letren}{0.8cm} 
              \setlength{\leduoi}{0.8cm} 
              \setlength{\letrai}{1.8cm} 
              \setlength{\lephai}{1cm} 
              \newgeometry{paperheight=27cm,paperwidth=19cm,left=\letrai,right=\lephai,top=\letren,bottom=\leduoi, footskip=24pt,headsep=6pt, includeheadfoot} 
            }{
                \ifthenelse{\equal{\MAPClass@pagesize}{Legal}}{
                    %\geometry{legalpaper}
                }{
                    \ifthenelse{\equal{\MAPClass@pagesize}{custom}}{
                        %\geometry{paperwidth=\MAPClass@width, paperheight=\MAPClass@height}
                    }{
                        %\geometry{a4paper} % Mặc định nếu không khớp với giá trị nào
                    }
                }
            }
        }
    }
		\ifthenelse{\equal{\MAPClass@company}{MAP}}{		   
		}
		{
			\ifthenelse{\equal{\MAPClass@company}{CTE}}{		   
			}
			{
				\ifthenelse{\equal{\MAPClass@company}{TQT}}{		   
				}
				{
				}
			}
		}
    \ifthenelse{\equal{\MAPClass@mausac}{01}}{}
    {
      \renewcommand{\Chonmau}[1]{
        \ifnum#1=01
          \colorlet{Mapcolor}{colorA}
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else\ifnum#1=02
          \colorlet{Mapcolor}{colorB}
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else\ifnum#1=03
          \colorlet{Mapcolor}{colorC}
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else\ifnum#1=04
          \colorlet{Mapcolor}{colorD}
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else\ifnum#1=05
          \colorlet{Mapcolor}{colorE}
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else\ifnum#1=06
          \colorlet{Mapcolor}{colorF}
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else\ifnum#1=07
          \colorlet{Mapcolor}{colorG}
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else\ifnum#1=08
          \colorlet{Mapcolor}{colorG}
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else\ifnum#1=09
          \colorlet{Mapcolor}{colorY}
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else\ifnum#1=10
          \colorlet{Mapcolor}{colorK} 
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else\ifnum#1=11
          \colorlet{Mapcolor}{colorL} 
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else\ifnum#1=12
          \colorlet{Mapcolor}{colorM} 
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else\ifnum#1=13
          \colorlet{Mapcolor}{colorN} 
          \colorlet{ViduG}{Mapcolor!30}          
          \sbox{\oldsqsymbol}{%
            \begin{tikzpicture}[baseline=-0.8ex]
              \Logomap
            \end{tikzpicture}
          }
        \else
          \PackageError{myclass}{Mã màu bạn chọn không hợp lệ}
        \fi\fi\fi\fi\fi\fi\fi\fi\fi\fi\fi\fi\fi
      }
    }
  }%


%%%%%%%%%%%%%%%%%%%%%%
%============================ĐỊNH NGHĨA GÕ TẮT=====================================
  \DeclareRobustCommand{\Up}[1]{\MakeUppercase{#1}} 
  \DeclareRobustCommand{\bth}[1]{\ensuremath{\displaystyle{#1}}} 
  \RequirePackage{xspace}
  %\tcbuselibrary{xparse}
  \DeclareRobustCommand{\linepic}[2]{
  \tcbsidebyside[sidebyside,righthand width=3cm,blanker,nobeforeafter]{%
  \haidong{#1}
  }{%
  #2
  }
  } 
  \def\imagetop#1{\raisebox{-\height+\baselineskip}{#1}}
  \DeclareRobustCommand{\ct}[1]{\text{#1}}
  \DeclareRobustCommand{\notext}{\textcolor{white}{Không hiển thị}\par\noindent}
  \DeclareRobustCommand{\Finish}{\par\centerline{\color{Mapcolor}\rule{0.2\textwidth}{2pt}\textbf{ HẾT } \rule{0.2\textwidth}{2pt}}\vspace{\baselineskip}}
  
  %Lệnh song song
  %\DeclareSymbolFont{symbolsC}{U}{txsyc}{m}{n}
  %\DeclareMathSymbol{\varparallel}{\mathrel}{symbolsC}{9}
  %\DeclareMathSymbol{\parallel}{\mathrel}{symbolsC}{9}
  \RequirePackage{xspace}
  \def\A{\textbf{A}}
  \def\B{\textbf{B}}
  \def\C{\textbf{C}}
  \def\D{\textbf{D}}
  \RequirePackage{esvect}
  \DeclareRobustCommand{\vec}[1]{
    \ifmmode\ \overrightarrow{#1}\else\ensuremath{\overrightarrow{#1}}\xspace\fi} 
  \DeclareRobustCommand{\pso}[2]{\bth{\frac{#1}{#2}}\xspace}
  \DeclareRobustCommand{\angstrom}{\textup{\AA}\xspace}
  \DeclareRobustCommand{\microc}{%
    \ifmmode\ \mu C\else\ensuremath{\ \mu C}\xspace\fi}
  \DeclareRobustCommand{\microm}{%
    \ifmmode\ \mu m\else\ensuremath{\ \mu m}\xspace\fi}
  \DeclareRobustCommand{\nodaik}[1]{%
    \ifmmode\alpha = #1\ K^{-1}\else\ensuremath{\alpha = #1\ K^{-1}}\xspace\fi}  
  \DeclareRobustCommand{\nodaic}[1]{%
    \ifmmode\alpha = #1\ \ct{độ}^{-1}\else\ensuremath{\alpha = #1\ \ct{độ}^{-1}}\xspace\fi}   
  \DeclareRobustCommand{\microf}{%
    \ifmmode\ \mu F\else\ensuremath{\ \mu F}\xspace\fi}
  \DeclareRobustCommand{\microh}{%
    \ifmmode\ \mu H\else\ensuremath{\ \mu H}\xspace\fi}      
  \DeclareRobustCommand{\degree}{%
    \ifmmode^\circ\else\ensuremath{^\circ}\xspace\fi}
  \DeclareRobustCommand{\doC}{%
    \ifmmode^\circ C\else\ensuremath{^\circ C}\xspace\fi}
  \DeclareRobustCommand{\lido}{%
    \ifmmode x=A\cos(\omega t+\varphi)\else\ensuremath{x=A\cos(\omega t+\varphi)}\xspace\fi}
  \DeclareRobustCommand{\canps}[2]{%
    \ifmmode \sqrt{\frac{#1}{#2}}\else\ensuremath{\displaystyle{\sqrt{\frac{#1}{#2}}}}\xspace\fi}    
  \DeclareRobustCommand{\ohm}{%
    \ifmmode\ \Omega\else\ensuremath{\Omega}\xspace\fi}
  \DeclareRobustCommand{\pit}{%
    \ifmmode\pi t\else\ensuremath{\pi t}\xspace\fi}  
  \DeclareRobustCommand{\gttd}[1]{%
    \ifmmode\ g=#1\ m/s^2\else\ensuremath{g=#1\ m/s^2}\xspace\fi}
  \DeclareRobustCommand{\met}{%
    \ifmmode\ m\else \ m\xspace\fi}      
  \DeclareRobustCommand{\cm}{%
    \ifmmode\ cm\else \ cm\xspace\fi}        
  \DeclareRobustCommand{\bs}[2]{% 
    \ensuremath{\displaystyle{#1 = #2}}\xspace}
  \DeclareRobustCommand{\sola}{% 
    \begin{sol}\textbf{A} \end{sol}}  
  \DeclareRobustCommand{\Check}{\faCheck\xspace}       
  \DeclareRobustCommand{\Bookmark}{\faBookmark\xspace}  
  \def\cung#1{\buildrel \frown \over{#1}}
  \DeclareRobustCommand{\vec}{\vv}
  \def\overrightarrow{\vv}
  \def\g.#1.{\widehat{#1}}
  \DeclareRobustCommand{\chia}{\;\vdots\;} 
  \DeclareRobustCommand{\kchia}{\not\vdots\;\;} 
  \DeclareRobustCommand{\R}{\mathbb R} 
  \DeclareRobustCommand{\N}{\mathbb N} 
  \DeclareRobustCommand{\Z}{\mathbb Z} 
  \DeclareRobustCommand{\hoac}[1]{ 
    \left[\begin{aligned}#1\end{aligned}\right.}
  \DeclareRobustCommand{\heva}[1]{
    \left\{\begin{aligned}#1\end{aligned}\right.}
  \DeclareRobustCommand{\Hn}[3]{% 
    \ce{_{#1}^{#2}#3}\xspace}    
  \DeclareRobustCommand{\textbi}[1]{\textsl{\textbf{#1}}}
  \DeclareRobustCommand{\Textbox}[2]{\noindent\parbox{#1\textwidth}{#2}}
  \DeclareRobustCommand{\tile}{1} 
  \DeclareRobustCommand{\Picbox}[2]{\noindent\parbox{#1\textwidth}{\centering
    \begin{tikzpicture}[scale=1,thick,>=stealth']
      #2
    \end{tikzpicture}}}
  \DeclareRobustCommand{\Imabox}[2]{\noindent\parbox{#1\textwidth}{\centering
    \begin{tikzpicture}[scale=1,opacity=0,thick,>=stealth']
      #2
    \end{tikzpicture}}}	
    \newcommand{\cautn}{\noindent
    \textbf{\fontfamily{qag}\selectfont\textcolor{Mapcolor}{\faFileText~PHẦN I. Câu trắc nghiệm nhiều phương án lựa chọn.}}
    \setcounter{ex}{0}
    \setcounter{bt}{0}
    \normalfont
    }
    \newcommand{\cautf}{\noindent
    \fontfamily{qag}\selectfont\textbf{\textcolor{Mapcolor}{\faFileText~PHẦN II. Câu trắc nghiệm đúng sai.}}
    \setcounter{ex}{0}
    \setcounter{bt}{0}
    \normalfont
    }
    \newcommand{\cauda}{\noindent
    \fontfamily{qag}\selectfont\textbf{\textcolor{Mapcolor}{\faFileText~PHẦN III. Câu trắc nghiệm trả lời ngắn.}}
    \setcounter{ex}{0}
    \setcounter{bt}{0}
    \normalfont
    }
    \newcommand{\cauv}{\noindent
    \fontfamily{qag}\selectfont\textbf{Câu hỏi tự luận.~}
    \setcounter{ex}{0}
    \setcounter{bt}{0}
    \normalfont
    }
%%% KHAI BÁO ĐỊNH DẠNG KHUNG
  \definecolor{colorAccent1}{cmyk}{0.0, 0.3, 0.6, 0.0} % cam đào nhẹ
  \definecolor{colorMDark}{cmyk}{0.95, 0.50, 0.0, 0.75}

  \newenvironment{note}
  {\begin{tcolorbox}[
    enhanced jigsaw,breakable,pad at break*=1mm,
    colframe=Mapcolor, %nền (opacityback=0, %ko nền)
    %--Trc sau
    before skip=1mm,
    after skip=1mm,
    boxrule=1pt, %đường viền dày
    drop fuzzy shadow=gray, %đổ bóng
    left=1mm,right=1mm,top=1mm,bottom=1mm,
    %--cong góc
    sharp corners=northwest,
    arc is angular,
    arc=2mm,
    frame hidden,
    fonttitle=\color{white}\sffamily\bfseries\fontsize{12}{0}\selectfont,
    title={\color{white} \faBookmark~CHÚ Ý},
    colback=Mapcolor!20,
    opacityback=0.05,
    coltitle=red,
    attach boxed title to top left={xshift=0.15cm,yshift=-0.2cm},
    boxed title style={empty},
    underlay boxed title={
    \fill[Mapcolor,rounded corners = 2pt]
    ([xshift=-4pt]title.south west) rectangle ([xshift=4pt,yshift=-3pt]title.north east);
    },
    borderline={2pt}{0mm}{colorMDark,dashed},
    ]
    }
  {\end{tcolorbox}}
  \def\xegiay#1{\begin{tcolorbox}[enhanced,sharp corners,frame hidden,
    interior style={draw=blue,line width=1pt,left color=blue!0,right color=white,
    decoration={random steps,segment length=0.3cm,amplitude=.1cm},
    decorate},
    boxsep=0mm,top=1mm,halign=left,after skip=5mm,bottom=1mm,left=1mm,right=1mm,]		
    #1
    \end{tcolorbox}
    }
  \newtcolorbox{mynote}{% 
    enhanced, left=2pt, bottom=1.5pt, top=1.5pt, breakable, 
    colback=yellow!10, colbacktitle=yellow!10, colframe=yellow!10, 
    leftrule = 1pt, rightrule=0pt, toprule=0pt, 
    sharp corners, 
    fonttitle=\fontfamily{qag}\bfseries, 
    title=\textcolor{Mapcolor}{\hspace{0.8cm}Chú ý}, 
    overlay={\fill[Mapcolor]($(frame.north west)+(0.4,0)$)--++(-90:4mm)--++(-45:3mm)--++(45:3mm)|-cycle;}, drop fuzzy shadow=gray!50 }
  \newtcolorbox{nx}{% 
    enhanced, left=2pt, bottom=1.5pt, top=1.5pt, breakable, 
    colback=Mapcolor!10, colbacktitle=Mapcolor!10, colframe=Mapcolor!10, 
    leftrule = 1pt, rightrule=0pt, toprule=0pt, 
    sharp corners, 
    fonttitle=\fontfamily{qag}\bfseries, 
    title=\textcolor{Mapcolor}{\hspace{0.8cm}Nhận xét:}, 
    overlay={\fill[Mapcolor]($(frame.north west)+(0.4,0)$)--++(-90:4mm)--++(-45:3mm)--++(45:3mm)|-cycle;}, drop fuzzy shadow=gray!50 } 
  \newtcolorbox{tc}{% 
    enhanced, left=2pt, bottom=1.5pt, top=1.5pt, breakable, 
    colback=Mapcolor!10, colbacktitle=Mapcolor!10, colframe=Mapcolor!10, 
    leftrule = 1pt, rightrule=0pt, toprule=0pt, 
    sharp corners, 
    fonttitle=\fontfamily{qag}\bfseries, 
    title=\textcolor{Mapcolor}{\hspace{0.8cm}Tính chất:}, 
    overlay={\fill[Mapcolor]($(frame.north west)+(0.4,0)$)--++(-90:4mm)--++(-45:3mm)--++(45:3mm)|-cycle;}, drop fuzzy shadow=gray!50 } 
  \newtcolorbox{hinhanh}[1]{% 
    enhanced jigsaw,  breakable,
    width = #1cm, 
    nobeforeafter,
    halign=center,
    top=0pt, left=1pt, bottom=0pt,right=1pt,
    colback=white!10, colbacktitle=white!10, colframe=white!10, 
    opacityback=0,
    leftrule = 1pt, rightrule=0pt, toprule=0pt, 
  } 
  \newtcolorbox{boxkn}{enhanced jigsaw,breakable,pad at break*=1.5mm,
  %		drop fuzzy shadow=orange,
    colback=Mapcolor!5,boxrule=0pt,frame hidden, left=4mm, right=0pt, bottom=1.5pt, top=1.5pt,
    before skip=2mm,
    after skip=2mm,
    %		borderline west={1mm}{0cm}{orange}
    overlay={\draw[line width=1pt,Mapcolor] ([xshift=1.5pt]interior.north west)--([xshift=1.5pt]interior.south west);
    \draw[line width=2pt,Mapcolor!10] ([xshift=0.1pt]interior.north east)--([xshift=0.1pt]interior.south east);
    \draw[line width=1pt,Mapcolor] ([xshift=-0.2pt]interior.south east)--([xshift=1.5pt]interior.south west);
    \draw[line width=2pt,Mapcolor!10] ([xshift=1.1pt]interior.north east)--([xshift=2pt]interior.north west);}
  }
  \newtcolorbox{boxdn}{enhanced jigsaw,breakable,pad at break*=1mm,
    %		drop fuzzy shadow=orange,
    colback=Mapcolor!5,boxrule=0pt,frame hidden, left=2mm, right=0pt, bottom=1.5pt, top=1.5pt,
    before skip=2mm,
    after skip=2mm,
    borderline west={1mm}{0cm}{Mapcolor!70}}
  \newtcolorbox{boxdl}{enhanced jigsaw,breakable,pad at break*=1mm,
    colback=Mapcolor!5,boxrule=0pt,frame hidden, left=2mm, right=0pt, bottom=1.5pt, top=1.5pt,
    before skip=2mm,
    after skip=2mm,
    borderline west={1mm}{0cm}{Mapcolor!70}}
  \providecommand{\itemKN}{\color{Mapcolor}\faCheckCircle~}
  \newtheorem{dn}{\color{Mapcolor}\faToggleOn~Định nghĩa}
  \tcolorboxenvironment{dn}{
    enhanced jigsaw,breakable,pad at break*=1mm,
    %		drop fuzzy shadow=orange,
    colback=Mapcolor!5,boxrule=0pt,frame hidden, left=2mm, right=0pt, bottom=1.5pt, top=1.5pt,
    before skip=2mm,
    after skip=2mm,
    borderline west={1mm}{0cm}{Mapcolor!70}}
    \newtheorem{dl}{\color{Mapcolor}\faToggleOn~Định lí}
    \tcolorboxenvironment{dl}{
      enhanced jigsaw,breakable,pad at break*=1mm,
      %		drop fuzzy shadow=orange,
      colback=Mapcolor!5,boxrule=0pt,frame hidden, left=2mm, right=0pt, bottom=1.5pt, top=1.5pt,
      before skip=2mm,
      after skip=2mm,
      borderline west={1mm}{0cm}{Mapcolor!70}}
  \newtcolorbox[auto counter]{khung4}[1]{enhanced, breakable,
		before skip=1mm,after skip=1mm,
		left=1mm,right=1mm,top=2mm,bottom=1mm,
		colframe=Mapcolor,colback=cyan!0,colbacktitle=cyan!6,coltitle=Mapcolor,colupper=black,sharp corners,
		,boxrule=0.4mm,
		attach boxed title to top center=
		{yshift=-0.1mm-\tcboxedtitleheight/2,yshifttext=2mm-\tcboxedtitleheight/2},
		varwidth boxed title*=-3cm,
		boxed title style={boxrule=0.3mm,
			frame code={ \path[tcb fill frame] ([xshift=-4mm]frame.west)
				-- (frame.north west) -- (frame.north east) -- ([xshift=4mm]frame.east)
				-- (frame.south east) -- (frame.south west) -- cycle; },
			interior code={ \path[tcb fill interior] ([xshift=-2mm]interior.west)
				-- (interior.north west) -- (interior.north east)
				-- ([xshift=2mm]interior.east) -- (interior.south east) -- (interior.south west)
				-- cycle;} 
		},
		fonttitle=\sffamily\fontsize{10}{0}
		\bfseries,
		fontupper=\sffamily\fontsize{11}{0},
		title={#1}
	}
  \definecolor{mybrown}{RGB}{33,34,28}
  \definecolor{myyellow}{RGB}{242,226,149}
  \DeclareTColorBox[auto counter,list inside=listtcb72]{luuy}{O{Mapcolor!10} d"" O{}}
  {
    enhanced jigsaw,
    frame code=empty,
    boxsep=0.5cm,
    top=0.4cm,
    colback=yellow!5,
    opacityback=0.5,
    left = 1pt,
    top=1pt,
    right=1pt,
    bottom=1pt,
  IfValueTF={#2}{watermark text={#2}}{},
    interior code={
    \fill[#1,drop shadow]
      (interior.south west) {[rounded corners=1.5cm]--
      (interior.south east)} --
      (interior.north east) --
      (interior.north west) --
      cycle;
    \fill[Mapcolor!80!#1] 
      ([shift={(-0.9cm,0.5ex)}]interior.south east) to[out=30,in=-70]
      ([shift={(-0.55cm,0.7cm)}]interior.south east) to[out=-10,in=220]
      ([shift={(-0.7ex,0.9cm)}]interior.south east) to[out=250,in=30]
      cycle;  
    \fill[Mapcolor]
      ([yshift=-10pt]interior.north) circle [radius=0.23cm];
    \fill[yellow!40]
      ([yshift=-10pt]interior.north) circle [radius=0.175cm];
    \node[
      cylinder,
      cylinder uses custom fill, 
      cylinder end fill=Mapcolor!60,
      cylinder body fill=Mapcolor,
      rotate=115,
      minimum width=9pt,
      minimum height=16pt,
    ] 
    at ([shift={(-0.2pt,-9.8pt)}]interior.north) {};
    },
  %title={#2},list entry={#2},
    #3}
    
  \newtheorem{pp}{\color{Mapcolor}\faLightbulbO\, Phương pháp}
	%---Dạng 1
	\newtcolorbox{dang}[1]{
		fonttitle=\fontfamily{qag}\bfseries,%fontupper=\itshape,
		breakable, 
		enhanced jigsaw, %nâng cao
		%	frame hidden, opacityback=0,
		colbacktitle=Mapcolor,
		coltitle=white,
		colframe=Mapcolor,
		colback=white!5,
    opacityback=0.5,
		sharp corners,
		before skip=3mm,after skip=3mm,
		left=2mm,right=2mm,top=2mm,bottom=2mm,
		%	boxrule=1pt,
		%--cong góc
		rounded corners=west, 
		arc=0mm,
		rounded corners=north, 
		rounded corners=northwest,
		arc=2mm,
		rounded corners=southeast,
		%	arc is angular,
		arc=0mm,
		underlay unbroken and first={
			\def\kctrai{1.5}
			\def\daikhung{1}
			%--Nền số section
			\fill[Mapcolor!60] 
			([xshift=\kctrai cm,yshift=0.12cm+0.1cm]title.north west) coordinate (bai1)%trên trái
			--++(\daikhung,0) coordinate (bai2)%trên phải
			[rounded corners=6pt]--++(0,-0.6) coordinate (bai3)%dưới phải
			--++(-\daikhung,0) coordinate (bai4)%dưới trái
			[rounded corners=0pt]--cycle
			;
			%--Bóng góc trái trên của khung
			\fill[gray!50] (bai1)--++(-0.2,-0.15)--++(0.2,0)--cycle;
			%--Bài và số bài
			\node[circle,draw=white,fill=Mapcolor, line width=1pt, inner sep=3pt,text=white] (SOBAI) at ([yshift=1pt]$(bai4)!0.5!(bai3)$) {\fontsize{12pt}{1pt}\fontfamily{put}\selectfont\bfseries \thedang};
			\draw ([yshift=0pt]SOBAI.west) node[left,white,scale=1] {\fontsize{12pt}{1pt}\fontfamily{qag}\selectfont\bfseries Dạng\,};	
		},
		title={%\faFolderOpen\ Dạng~\stepcounter{dang}\thedang.\ 
			\hspace*{2.5cm}\parbox{\textwidth-2.75cm}{\textcolor{white}{#1}}\stepcounter{dang}\setcounter{subsubsection}{0}\setcounter{vidu}{0}\setcounter{ex}{0}\setcounter{bt}{0}}%
		\addcontentsline{toc}{subsection}{\vspace{.15cm}\sffamily \faFolderOpen\ Dạng~\thedang. #1}
    \setcounter{vidu}{0} \setcounter{vd}{0}
	}
  \newtcolorbox{dang*}[1]{
		fonttitle=\fontfamily{qag}\bfseries,%fontupper=\itshape,
		breakable, 
		enhanced jigsaw, %nâng cao
		%	frame hidden, opacityback=0,
		colbacktitle=Mapcolor,
		coltitle=white,
		colframe=Mapcolor,
		colback=white!5,
    opacityback=0.5,
		sharp corners,
		before skip=3mm,after skip=3mm,
		left=2mm,right=2mm,top=2mm,bottom=2mm,
		%	boxrule=1pt,
		%--cong góc
		rounded corners=west, 
		arc=0mm,
		rounded corners=north, 
		rounded corners=northwest,
		arc=2mm,
		rounded corners=southeast,
		%	arc is angular,
		arc=0mm,
		underlay unbroken and first={
			\def\kctrai{1.5}
			\def\daikhung{1}
			%--Nền số section
			\shade[Mapcolor!60] 
			([xshift=\kctrai cm,yshift=0.15cm]title.north west) coordinate (bai1)%trên trái
			--++(\daikhung,0) coordinate (bai2)%trên phải
			[rounded corners=6pt]--++(0,-0.6) coordinate (bai3)%dưới phải
			--++(-\daikhung,0) coordinate (bai4)%dưới trái
			[rounded corners=0pt]--cycle
			;
			%--Bóng góc trái trên của khung
			\fill[gray!50] (bai1)--++(-0.2,-0.15)--++(0.2,0)--cycle;
			%--Bài và số bài
			\node[circle,draw=white,fill=Mapcolor, line width=1pt, inner sep=3pt,text=white] (SOBAI) at ([yshift=1pt]$(bai4)!0.5!(bai3)$) {\fontsize{12pt}{1pt}\fontfamily{put}\selectfont\bfseries \thedang};
			\draw ([yshift=-2pt]SOBAI.west) node[left,white,scale=1] {\fontsize{12pt}{1pt}\fontfamily{qag}\selectfont\bfseries Dạng\,};	
		},
    lowerbox=ignored,
    frame hidden,
    top=-\baselineskip,
		title={%\faFolderOpen\ Dạng~\stepcounter{dang}\thedang.\ 
			\hspace*{2.5cm}\parbox{\textwidth-2.75cm}{\textcolor{white}{\MakeUppercase{#1}}}\stepcounter{dang}\setcounter{subsubsection}{0}\setcounter{vidu}{0}\setcounter{ex}{0}\setcounter{bt}{0}}%
		\addcontentsline{toc}{subsection}{\vspace{.15cm}\sffamily \faFolderOpen\ Dạng~\thedang. \MakeUppercase{#1}}
		 \setcounter{vidu}{0} \setcounter{vd}{0}
	} 
\newcommand{\caulc}{
	\begin{flushleft}
		\color{orange!80!red!75!black}	\fbox{ \fontfamily{qag}\bfseries\selectfont PHẦN 1. Câu trắc nghiệm 4 phương án}
	\end{flushleft}
	\setcounter{ex}{0}
}

\newcommand{\cauds}{
	\begin{flushleft}
		\color{orange!80!red!75!black}	\fbox{ \fontfamily{qag}\bfseries\selectfont PHẦN 2. Câu trắc nghiệm đúng sai}
	\end{flushleft}
	\setcounter{ex}{0}
}
\newcommand{\caukq}{
	\begin{flushleft}
		\color{orange!80!red!75!black}	\fbox{ \fontfamily{qag}\bfseries\selectfont PHẦN 3. Câu trắc nghiệm trả lời ngắn.}
	\end{flushleft}
	\setcounter{ex}{0}
}
  \DeclareRobustCommand{\Note}[1]{\par\noindent\textcolor{Mapcolor}{\faPencilSquareO\ \textbf{Chú ý:} #1}}
  \newcommand{\newghichu}[1]{\begingroup
  \begin{tcolorbox}[%
      enhanced jigsaw,
      breakable,
      before skip=2mm,
      after skip=2mm,
      left=1mm, right=0mm, top=0mm, bottom=0mm,
      colback=yellow!20,
      borderline west={0.8mm}{0pt}{Mapcolor},arc=0mm,
      leftrule=2pt, toprule=0pt, rightrule=0pt,bottomrule=0pt,
      colframe=white,
      opacityback=0.5,
      grow to left by=-1.5cm,
      overlay={%
      \node[anchor=north west] 
        at ([xshift=-1.3cm,yshift=-.15\baselineskip]interior.north west)
      {\huge\bfseries \color{Mapcolor}};
          }
    ]
    #1
  \end{tcolorbox}
  \endgroup}
  \newtcolorbox{Ghichu}[1][]{colback=white,colframe=Mapcolor,fonttitle=\bfseries,
    title=Em có biết?,#1}					
  \newcommand\Binhluan[2]{\begingroup
  \noindent\begin{minipage}[t]{0.68\textwidth}
  #1
  \end{minipage}\hfill
  \begin{minipage}[t]{0.3\textwidth}\vspace{0.2\baselineskip}
  \begin{Ghichu}
  #2
  \end{Ghichu}
  \end{minipage}
  \endgroup
  }
  \newtcolorbox{bang}[2][]{%
    breakable,
    enhanced jigsaw,
    colback=Mapcolor,
    coltitle=white,
    fonttitle={\bfseries\large},
    fontlower=\normalsize,
    left=1pt,
    right=1pt,
    bottom=1pt,
    top=1pt,
    title={#2},
    colframe=Mapcolor,
    opacityback=0.2,
    colback=yellow!30,
    center title,
    #1
  } 
  \newtcolorbox{bangchoice}[2][]{%
    breakable,
    enhanced jigsaw,
    colback=Mapcolor,
    coltitle=white,
    fonttitle={\bfseries\large},
    fontlower=\normalsize,
    left=1pt,
    right=1pt,
    bottom=1pt,
    top=1pt,
    nobeforeafter,
    title={#2},
    colframe=Mapcolor,	
    colback=yellow!30,
    opacityback=0,
    center title,
    #1
  }   
  \newcounter{bangdethi}\setcounter{bangdethi}{0}
  \renewcommand{\thebangdethi}{\arabic{bangdethi}}
  \newtcolorbox{bangDA}[1][]{colback=red!5!white,
    colframe=Mapcolor,fonttitle=\bfseries,
    colbacktitle=Mapcolor,enhanced,breakable,
    left=1pt,
    right=1pt,
    bottom=1pt,
    top=1pt,
    nobeforeafter,
    attach boxed title to top center={yshift=-3mm},
    title={BẢNG ĐÁP ÁN ĐỀ SỐ ~\stepcounter{bangdethi}\thebangdethi},#1
  }
  \AtBeginEnvironment{bangDA}{\setcounter{subsubsection}{0}}
  \newcommand{\ghichu}[1]{\begingroup
  \begin{tcolorbox}[%
      enhanced jigsaw,
      breakable,
      before skip=2mm,
      after skip=2mm,
      left=1mm, right=0mm, top=6mm, bottom=1mm,
      colback=yellow!20,
      %borderline west={0.8mm}{0pt}{Mapcolor},arc=0mm,
      leftrule=0pt, toprule=0pt, rightrule=0pt,bottomrule=0pt,
      colframe=Mapcolor!30,
      opacityback=0.5,
      grow to left by=-0.5cm,
      overlay={%
    \begin{scope}
        %Vẽ đoạn thẳng từ góc trên bên trái đến góc dưới bên trái
      \draw[line width=1pt,Mapcolor] 
              ([xshift=0.1cm,yshift=-0.7cm]frame.north west) -- 
              ([xshift=0.1cm,yshift=0.1cm]frame.south west);
        %Vẽ chấm tròn ở hai đầu (góc trên bên trái và dưới bên trái)
      \fill[draw=Mapcolor!30, fill=Mapcolor] ([xshift=0.1cm,yshift=-0.7cm]frame.north west) circle[radius=0.5mm];
          \fill[draw=Mapcolor!30, fill=Mapcolor] ([xshift=0.1cm,yshift=0.1cm]frame.south west) circle[radius=0.5mm];
      \end{scope}
      \node[anchor=north west] 
        at ([xshift=-0.5cm,yshift=0.5\baselineskip]interior.north west)
      {\huge\bfseries \color{Mapcolor}\faPaperclip};
    \node[anchor=north west] 
        at ([xshift=0.3cm,yshift=0\baselineskip]interior.north west)
      {\large\bfseries \color{Mapcolor}Ghi chú:};
          }
    ]
    #1
  \end{tcolorbox}
  \endgroup}
  \newcommand{\emcobiet}[1]{\begingroup
\begin{tcolorbox}[%
    enhanced jigsaw,
    breakable,
    before skip=2mm,
    after skip=2mm,
    left=-6mm, right=3mm, top=8mm, bottom=3mm,
    colback=white,
    %borderline west={0.8mm}{0pt}{Mapcolor},arc=5mm,
    leftrule=0pt, toprule=0pt, rightrule=0pt,bottomrule=0pt,
     colframe=white,
     opacityback=0.5,
     grow to left by=-1.5cm,
     overlay={%
	 \begin{scope}
			%Vẽ nền của khung
		\fill[Mapcolor!60, rounded corners=10pt,opacity=0.15]([xshift=-1cm,yshift=-0.6cm]frame.north west) -- ([xshift=0cm,yshift=-0.6cm]frame.north east)[rounded corners=5pt]-- ([xshift=0cm]frame.south east)[rounded corners=5pt]-- ([xshift=-1cm]frame.south west)[rounded corners=5pt]--cycle;
     \end{scope}
	\node[anchor=north west,scale=0.7] 
      at ([xshift=-1.35cm,yshift=-0.15cm]interior.north west)
    {\huge\bfseries \color{Mapcolor!70}\faQuestion};
    \node[anchor=north west] 
      at ([xshift=-1.2cm,yshift=0.05cm]interior.north west)
    {\huge\bfseries \color{Mapcolor}\faQuestion};
	\node[anchor=north west] 
      at ([xshift=-0.5cm,yshift=0.15\baselineskip]interior.north west)
    {\large\bf\fontfamily{qag}\selectfont \color{Mapcolor}EM CÓ BIẾT};
         }
	,coltext=Mapcolor]#1
\end{tcolorbox}
\endgroup}
\newcommand{\mobai}[1]{\begingroup
\begin{tcolorbox}[%
    enhanced jigsaw,
    breakable,
    before skip=2mm,
    after skip=2mm,
    left=-6mm, right=3mm, top=8mm, bottom=3mm,
    colback=white,
    leftrule=0pt, toprule=0pt, rightrule=0pt,bottomrule=0pt,
     colframe=white,
     opacityback=0.5,
     grow to left by=-1.5cm,
     overlay={%
	 \begin{scope}
			%Vẽ nền của khung
		\fill[cyan, rounded corners=10pt,opacity=0.15]([xshift=-1cm,yshift=-0.6cm]frame.north west) -- ([xshift=0cm,yshift=-0.6cm]frame.north east)[rounded corners=4pt]-- ([xshift=0cm]frame.south east)[rounded corners=4pt]-- ([xshift=-1cm]frame.south west)[rounded corners=4pt]--cycle;
		\draw[Mapcolor, rounded corners=4pt,line width=2pt]([xshift=-1cm,yshift=-0.6cm]frame.north west) -- ([xshift=0cm,yshift=-0.6cm]frame.north east)[rounded corners=4pt]-- ([xshift=0cm]frame.south east)[rounded corners=4pt]-- ([xshift=-1cm]frame.south west)[rounded corners=4pt]--cycle;
     \end{scope}
	\node[anchor=north west] 
      at ([xshift=-1.61cm,yshift=0.05cm]interior.north west)
    {\Huge\bfseries \color{Mapcolor}\scalebox{1.25}{\faCircle}};
	\node[anchor=north west] 
      at ([xshift=-1.46cm,yshift=-0.05cm]interior.north west)
    {\huge\bfseries \color{white}\faPowerOff};
         }
	,coltext=black]#1
\end{tcolorbox}
\endgroup}
\newcommand{\lichsu}[1]{\begingroup
\begin{tcolorbox}[%
    enhanced jigsaw,
    breakable,
    before skip=2mm,
    after skip=2mm,
    left=-6mm, right=3mm, top=8mm, bottom=3mm,
    colback=white,
    %borderline west={0.8mm}{0pt}{Mapcolor},arc=5mm,
    leftrule=0pt, toprule=0pt, rightrule=0pt,bottomrule=0pt,
     colframe=white,
     opacityback=0.5,
     grow to left by=-1.5cm,
     overlay={%
	 \begin{scope}
			%Vẽ nền của khung
		\fill[cyan, rounded corners=10pt,opacity=0.15]([xshift=-1cm,yshift=-0.6cm]frame.north west) -- ([xshift=0cm,yshift=-0.6cm]frame.north east)[rounded corners=10pt]-- ([xshift=0cm]frame.south east)[rounded corners=10pt]-- ([xshift=-1cm]frame.south west)[rounded corners=10pt]--cycle;
		\draw[Mapcolor!30, rounded corners=10pt,line width=2pt]([xshift=-1cm,yshift=-0.6cm]frame.north west) -- ([xshift=0cm,yshift=-0.6cm]frame.north east)[rounded corners=10pt]-- ([xshift=0cm]frame.south east)[rounded corners=10pt]-- ([xshift=-1cm]frame.south west)[rounded corners=10pt]--cycle;
     \end{scope}
	\node[anchor=north west] 
      at ([xshift=-1.6cm,yshift=-0.05cm]interior.north west)
    {\Huge\bfseries \color{Mapcolor!30}\scalebox{1.2}{\faCircle}};
	\node[anchor=north west] 
      at ([xshift=-1.46cm,yshift=-0.19cm]interior.north west)
    {\huge\bfseries \color{white}\faHistory};
         }
	,coltext=cyan!50!blue]#1
\end{tcolorbox}
\endgroup}
\newcommand{\ungdung}[1]{\begingroup
\begin{tcolorbox}[%
    enhanced jigsaw,
    breakable,
    before skip=2mm,
    after skip=2mm,
    left=-6mm, right=3mm, top=8mm, bottom=3mm,
    colback=white,
    %borderline west={0.8mm}{0pt}{Mapcolor},arc=5mm,
    leftrule=0pt, toprule=0pt, rightrule=0pt,bottomrule=0pt,
     colframe=white,
     opacityback=0.5,
     grow to left by=-1.5cm,
     overlay={%
	 \begin{scope}
			%Vẽ nền của khung
		\fill[cyan, rounded corners=10pt,opacity=0.15]([xshift=-1cm,yshift=-0.6cm]frame.north west) -- ([xshift=0cm,yshift=-0.6cm]frame.north east)[rounded corners=15pt]-- ([xshift=0cm]frame.south east)[rounded corners=10pt]-- ([xshift=-1cm]frame.south west)[rounded corners=10pt]--cycle;
		\draw[Mapcolor!30, rounded corners=10pt,line width=2pt]([xshift=-1cm,yshift=-0.6cm]frame.north west) -- ([xshift=0cm,yshift=-0.6cm]frame.north east)[rounded corners=10pt]-- ([xshift=0cm]frame.south east)[rounded corners=10pt]-- ([xshift=-1cm]frame.south west)[rounded corners=10pt]--cycle;
     \end{scope}
	\node[anchor=north west] 
      at ([xshift=-1.5cm,yshift=-0.05cm]interior.north west)
    {\huge\bfseries \color{Mapcolor!30}\scalebox{1.25}{\faSquare}};
	\node[anchor=north west] 
      at ([xshift=-1.41cm,yshift=-0.13cm]interior.north west)
    {\huge\bfseries \color{white}\faForward};
         }
	,coltext=cyan!50!blue]#1
\end{tcolorbox}
\endgroup}
% ----------------------------
	% Bộ đếm cho các Phần
	% ----------------------------
	\newcounter{phan}

	% Macro chung cho mọi phần, La Mã tự động
	\DeclareRobustCommand{\Phan}[1]{%
	  \refstepcounter{phan}% hoặc \stepcounter{phan} nếu không cần \label
	  \medskip\noindent\fontfamily{qag}\selectfont
	  \textbf{\color{Mapcolor}Phần \Roman{phan}: #1}\noindent
	  \setcounter{ex}{0}\setcounter{bt}{0}%
	  \normalfont\relax
	}

	% Các alias cho từng loại phần
	\DeclareRobustCommand{\PhanI}{\setcounter{phan}{0}\Phan{CÂU HỎI TRẮC NGHIỆM. Mỗi câu học sinh chỉ chọn một phương án}}
	\DeclareRobustCommand{\PhanII}{\Phan{CÂU HỎI TRẮC NGHIỆM ĐÚNG SAI. Trong mỗi ý a), b), c), d) học sinh chọn đúng hoặc sai.}}
	\DeclareRobustCommand{\PhanIII}{\Phan{CÂU HỎI TRẢ LỜI NGẮN. Học sinh ghi đáp án vào ô kết quả}}
	\DeclareRobustCommand{\PhanTL}{\Phan{CÂU HỎI TỰ LUẬN. Học sinh trình bày chi tiết lời giải}}
	\DeclareRobustCommand{\PhanIV}{\Phan{CÂU HỎI TỰ LUẬN. Học sinh trình bày chi tiết lời giải}}
  \RequirePackage{dashrule}
  \flushbottom

  % Định nghĩa lại lệnh \haidong để sử dụng các dòng gạch ngang liền nét mờ đi 30%
  \newcommand{\haidong}[1]{
    \def\numlinedot{#1}
    \par
    \foreach \dotline in{1,...,\numlinedot}{
      \noindent\Linefont\color{Mapcolor!50}\hrulefill % Sử dụng lệnh hrulefill để tạo dòng gạch ngang liền nét
      \par
    }
  }
  \newcommand{\badong}[1]{
    \def\numlinedot{#1}
    \par
    \foreach \dotline in{1,...,\numlinedot}{
      \noindent\Linefont\color{Mapcolor!50}\hrulefill % Sử dụng lệnh hrulefill để tạo dòng gạch ngang liền nét
      \par
    }
  }
  \newcommand{\oneline}{
    \noindent\Linefont\dotfill
  }
  %============================Ô TÍCH TRẢ LỜI NGẮN===================================
% Lệnh cho ô vuông đánh dấu đúng
  \newcommand{\traloingan}{\begin{tikzpicture}
    %\fill[left color = blue!50,right color = red!50, middle color=blue!50!black,rounded corners=2pt](0,0)--++(0:2)--++(90:0.5)--++(180:2)--cycle;
    \draw[green!40!blue,rounded corners=2pt,thick](0,0)--++(0:2)--++(90:0.5)--++(180:2)--cycle;
    \draw[green!40!blue, thick](0.5,0)--(0.5,0.5)(1,0)--(1,0.5)(1.5,0)--(1.5,0.5);
  \end{tikzpicture}
  }
  \newcommand{\tln}{\begin{tikzpicture}
    %\fill[left color = blue!50,right color = red!50, middle color=blue!50!black,rounded corners=2pt](0,0)--++(0:2)--++(90:0.5)--++(180:2)--cycle;
    \draw[green!40!blue,rounded corners=2pt,thick](0,0)--++(0:2)--++(90:0.5)--++(180:2)--cycle;
    \draw[green!40!blue, thick](0.5,0)--(0.5,0.5)(1,0)--(1,0.5)(1.5,0)--(1.5,0.5);
  \end{tikzpicture}
  }

  \newcommand{\nguon}[1]{(#1)}
  \newcommand{\gv}[1]{\par\noindent\large{\textbf{\textcolor{red}{GV chú ý: #1}}}\normalsize}
  \DeclareRobustCommand{\loai}[1]{\par\noindent\textbf{Loại: }#1}
  \newcommand{\Anlythuyet}[1]{#1}
  \RequirePackage{makecell}
  \RequirePackage{tabularx}
	\newcolumntype{Y}{>{\raggedleft\arraybackslash}X}
	\newcolumntype{M}{>{\centering\arraybackslash}X}
  \RequirePackage{booktabs, array, rotating, varwidth}
  \newbox\LT@firstfoot
  \def\endfirstfoot{\LT@end@hd@ft\LT@firstfoot}
  \newdimen\LT@footdiff
  \def\LT@start{%
    \let\LT@start\endgraf
    \endgraf\penalty\z@
    \vskip\LTpre
    \LT@footdiff-\ht\LT@foot
    \advance\LT@footdiff\ht\LT@firstfoot
    \dimen@\pagetotal
    \advance\dimen@ \ht\ifvoid\LT@firsthead\LT@head\else\LT@firsthead\fi
    \advance\dimen@ \dp\ifvoid\LT@firsthead\LT@head\else\LT@firsthead\fi
    \advance\dimen@ \ht\ifvoid\LT@firstfoot\LT@foot\else\LT@firstfoot\fi
    \dimen@ii\vfuzz
    \vfuzz\maxdimen
    \setbox\tw@\copy\z@
    \setbox\tw@\vsplit\tw@ to \ht\@arstrutbox
    \setbox\tw@\vbox{\unvbox\tw@}%
    \vfuzz\dimen@ii
    \advance\dimen@ \ht
        \ifdim\ht\@arstrutbox>\ht\tw@\@arstrutbox\else\tw@\fi
    \advance\dimen@\dp
        \ifdim\dp\@arstrutbox>\dp\tw@\@arstrutbox\else\tw@\fi
    \advance\dimen@ -\pagegoal
    \ifdim \dimen@>\z@\vfil\break\fi
    \global\@colroom\@colht
    \ifvoid\LT@firstfoot
      \ifvoid\LT@foot
      \else
        \advance\vsize-\ht\LT@foot
        \global\advance\@colroom-\ht\LT@foot
        \dimen@\pagegoal\advance\dimen@-\ht\LT@foot\pagegoal\dimen@
        \maxdepth\z@
      \fi
    \else
      \advance\vsize-\ht\LT@firstfoot
      \global\advance\@colroom-\ht\LT@firstfoot
      \dimen@\pagegoal\advance\dimen@-\ht\LT@firstfoot\pagegoal\dimen@
      \maxdepth\z@
    \fi
    \ifvoid\LT@firsthead\copy\LT@head\else\box\LT@firsthead\fi\nobreak
    \output{\LT@output}%
  }
  \def\LT@output{%
    \ifnum\outputpenalty <-\@Mi
      \ifnum\outputpenalty > -\LT@end@pen
        \LT@err{floats and marginpars not allowed in a longtable}\@ehc
      \else
        \setbox\z@\vbox{\unvbox\@cclv}%
        \ifdim \ht\LT@lastfoot>\ht\LT@foot
          \dimen@\pagegoal
          \advance\dimen@-\ht\LT@lastfoot
          \ifdim\dimen@<\ht\z@
            \setbox\@cclv\vbox{\unvbox\z@\copy\LT@foot\vss}%
            \@makecol
            \@outputpage
            \setbox\z@\vbox{\box\LT@head}%
          \fi
        \fi  
        \global\@colroom\@colht
        \global\vsize\@colht   
        \vbox
          {\unvbox\z@\box\ifvoid\LT@lastfoot\LT@foot\else\LT@lastfoot\fi}%
      \fi
    \else
      \ifvoid\LT@firstfoot
        \setbox\@cclv\vbox{\unvbox\@cclv\copy\LT@foot\vss}%
        \@makecol
        \@outputpage
        \global\vsize\@colroom
      \else
        \setbox\@cclv\vbox{\unvbox\@cclv\box\LT@firstfoot\vss}%
        \@makecol
        \@outputpage
        \global\advance\@colroom\LT@footdiff
        \global\vsize\@colroom
      \fi
      \copy\LT@head\nobreak
    \fi
  }
  %MỘT SỐ TIÊU ĐỀ BỔ SUNG===========================================================================
  \DeclareRobustCommand{\Slogan}[2]{\hspace*{2.5em}\textcolor{Mapcolor}{\textit{``#1''}}\par\hfill\textcolor{Mapcolor}{\textbf{(#2)}}}
  \DeclareRobustCommand{\boder}[1]{%
  \tcboxmath[standard jigsaw, colback=white, colframe=Mapcolor, lowerbox=invisible, opacityback=0, arc=1pt,left=1pt, right=1pt, bottom=1pt, top=1pt,grow to left by=-5mm]%
            {#1}}
  \DeclareRobustCommand{\NOTE}{\par\noindent \textbf{\textcolor{Mapcolor}{\faBook\ CÁC EM LƯU Ý\normalsize}}\par\noindent}
  \DeclareRobustCommand{\BTVN}{\par\noindent\Large{\textbf{\textcolor{Mapcolor}{CÂU HỎI LÍ THUYẾT}}}\normalsize}
  \DeclareRobustCommand{\BTTN}{\par\noindent\Large{\textbf{\textcolor{Mapcolor}{\ BÀI TẬP TRẮC NGHIỆM}}}\normalsize}
  \DeclareRobustCommand{\CHTN}{\par\noindent\Large\textbf{\textcolor{Mapcolor}{\ BÀI TẬP TRẮC NGHIỆM\ \titlerule[4pt]}}\normalsize}
  \DeclareRobustCommand{\btvn}{\par\centerline{\Large\textbf{\textcolor{coolblack}{\faGg\faGg\faGg\faGg\ BÀI TẬP VỀ NHÀ \faGg\faGg\faGg\faGg}}}}
  \DeclareRobustCommand{\BTVD}{\par\centerline{\myfont{13}{18}\fontfamily{qag}\selectfont\textbf{\textcolor{Mapcolor}{\faGg\faGg\faGg\faGg\ BÀI TẬP VẬN DỤNG \faGg\faGg\faGg\faGg}}}}
\DeclareRobustCommand{\btap}{\par\centerline{\Large\textbf{\textcolor{coolblack}{\faGg\faGg\faGg\faGg\ BÀI TẬP ÁP DỤNG \faGg\faGg\faGg\faGg}}}}
  \DeclareRobustCommand{\capdode}{\par\noindent\Large\textbf{\textcolor{Mapcolor}{\faAslInterpreting\ CẤP ĐỘ DỄ\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf}}}
  \DeclareRobustCommand{\capdovua}{\par\noindent\Large\textbf{\textcolor{Mapcolor}{\faAslInterpreting\ CẤP ĐỘ VỪA\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf}}}
  \DeclareRobustCommand{\capdokho}{\par\noindent\Large\textbf{\textcolor{Mapcolor}{\faAslInterpreting\ CẤP ĐỘ KHÓ\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf\ \faLeaf}}}
  \DeclareRobustCommand{\Vidumau}{\noindent\large\textbf{\textcolor{Mapcolor}{\ VÍ DỤ MINH HỌA}}\normalsize\setcounter{vidu}{0}}
  \DeclareRobustCommand{\Center}[1]{%
  \hfill\bth{#1}\hfill\null}  
  \DeclareRobustCommand{\Vehinh}[2]{%
  \begin{tikzpicture}[scale=#1,thick,>=stealth']
  #2
  \end{tikzpicture}
  }

%%%KHAI BÁO ĐẦU TÀI LIỆU=================================================
  \AtBeginDocument{%
  \normalsize
  \raggedbottom 
  \allowdisplaybreaks
  \setlength{\belowdisplayskip}{2pt} \setlength{\belowdisplayshortskip}{2pt}
  \setlength{\abovedisplayskip}{2pt} \setlength{\abovedisplayshortskip}{2pt}
  \everymath{\displaystyle}
  \renewcommand{\arraystretch}{1.5}
  \arrayrulecolor{Mapcolor} 
  \setlength{\arrayrulewidth}{1pt} 
  %\createbox{vidu}
}
\newenvironment{tomtat}{
	\begin{tcolorbox}[boxrule=1pt,arc=0mm,breakable]
	}{\end{tcolorbox}}
\newenvironment{name}[2]{
	\noindent
	\tikz \draw[double,blue!50!green] (0,0)--(\textwidth,0);
	\begin{minipage}{0.44\textwidth}
		\centering
		\color{blue!50!green}	{\bfseries\fontfamily{lmtt}\selectfont #1}\\
		\rule{3cm}{1pt}\\
		{\large\color{blue}	\bfseries\fontfamily{qag}\selectfont ĐỀ SỐ \circEX{\thedemsode}}
	\end{minipage}
	\begin{minipage}{0.54\textwidth}
		\color{blue!50!green}	\centering
		{\bfseries\fontfamily{lmtt}\selectfont \MakeUppercase{#2} }\\
		\addcontentsline{toc}{section}{Đề số \thedemsode.\, #2}
	\end{minipage}
	\\[0.5cm]\color{blue!50!green}\hspace*{1cm}	\textit{Họ và tên thí sinh:} \tikz \draw[dotted,thick] (0,0)--(\textwidth/3,0); 
	\textit{Số báo danh:} \EXboxTF{}\EXboxTF{}\EXboxTF{}\EXboxTF{}\EXboxTF{}\EXboxTF{}\EXboxTF{}\EXboxTF{}\\
	\tikz \draw[double,blue!50!green] (0,0)--(\textwidth,0);
	\stepcounter{demsode}
}

%indapan
\newcommand{\indapan}[2]{
	\begin{center}
		\color{blue!50!green}	\fbox{\fontfamily{qag}\bfseries\selectfont BẢNG ĐÁP ÁN}
	\end{center}
	{	\fontfamily{lmtt}\bfseries\selectfont	\inputansbox[3]{#1}{#2}}
}

\renewcommand{\indapan}{}
\newcommand{\cautl}{
	\begin{flushleft}
		\color{blue!50!green}	\fbox{ \fontfamily{qag}\bfseries\selectfont PHẦN 4. Câu tự luận}
	\end{flushleft}
}
\usepackage{array}
\usepackage{longtable}
\newcolumntype{C}[1]{>{\centering\arraybackslash}p{#1}}
\providecommand{\viduminhhoa}{\par\centerline{\bfseries\color{blue}VÍ DỤ MINH HỌA}\par\medskip}
\providecommand{\baitaptl}{\par\centerline{\bfseries\color{blue}BÀI TẬP TỰ LUẬN}\par\medskip}
\newtcolorbox{hq}{enhanced jigsaw,breakable,pad at break*=1mm,
    colback=Mapcolor!5,boxrule=0pt,frame hidden, left=2mm, right=0pt, bottom=1.5pt, top=1.5pt,
    before skip=2mm,
    after skip=2mm,
    borderline west={1mm}{0pt}{Mapcolor}}
\endinput

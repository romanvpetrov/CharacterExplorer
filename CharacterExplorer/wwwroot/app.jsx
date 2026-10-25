const { useEffect, useMemo, useState } = React;
const films = [
  ["The Phantom Menace", "I"], ["Attack of the Clones", "II"],
  ["Revenge of the Sith", "III"], ["A New Hope", "IV"],
  ["The Empire Strikes Back", "V"], ["Return of the Jedi", "VI"],
];

function Fact({ label, value }) {
  return <div className="fact"><label>{label}</label><b>{value || "unknown"}</b></div>;
}

function App() {
  const [people, setPeople] = useState([]), [query, setQuery] = useState(""),
    [selected, setSelected] = useState(null), [person, setPerson] = useState(null),
    [loading, setLoading] = useState(true), [error, setError] = useState("");

  useEffect(() => {
    fetch("/api/people").then((r) => r.ok ? r.json() : Promise.reject())
      .then((x) => { setPeople(x); setSelected(x[0]?.id); })
      .catch(() => setError("Unable to load the character index."))
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    if (!selected) return;
    setPerson(null);
    fetch("/api/people/" + selected).then((r) => r.ok ? r.json() : Promise.reject())
      .then(setPerson).catch(() => setError("Unable to retrieve this character."));
  }, [selected]);

  const filtered = useMemo(() => people.filter((p) =>
    p.name.toLowerCase().includes(query.toLowerCase())), [people, query]);

  return <>
    <header className="top">
      <div className="brand"><span aria-hidden="true">✦</span> HOLOCRON <small className="eyebrow">// character index</small></div>
      <div className="search"><span aria-hidden="true">⌕</span><input aria-label="Search characters" placeholder="Search characters" value={query} onChange={(e) => setQuery(e.target.value)} /></div>
    </header>
    <main className="layout">
      <aside className="side"><h3 className="eyebrow">Characters // {filtered.length}</h3><div className="side-list">
        {loading ? <div className="state">INDEXING...</div> : filtered.length === 0 ? <div className="state">NO MATCHES</div> : filtered.map((p) =>
          <button className={"person-button " + (selected === p.id ? "active" : "")} onClick={() => setSelected(p.id)} key={p.id}>{p.name}</button>)}
      </div></aside>
      <section className="main">
        {error && !person ? <div className="state error">{error}</div> : !person ? <div className="state">{loading ? "LOADING DATAPAD..." : "SELECT A CHARACTER"}</div> : <>
          <div className="card"><div className="hero"><div className="avatar" aria-hidden="true">✦</div><div><div className="eyebrow">Datapad // {String(person.id).padStart(3, "0")}</div><h1>{person.name}</h1></div></div>
            <div className="grid3"><Detail title="Attributes"><Fact label="Birth year" value={person.birthYear} /><Fact label="Gender" value={person.gender} /><Fact label="Height" value={person.height} /><Fact label="Mass" value={person.mass} /><Fact label="Eye color" value={person.eyeColor} /><Fact label="Hair color" value={person.hairColor} /><Fact label="Skin color" value={person.skinColor} /></Detail>
              <Detail title="Species">{person.species ? <><Fact label="Name" value={person.species.name} /><Fact label="Classification" value={person.species.classification} /><Fact label="Designation" value={person.species.designation} /><Fact label="Average height" value={person.species.averageHeight} /><Fact label="Lifespan" value={person.species.averageLifespan} /><Fact label="Language" value={person.species.language} /></> : <div className="state">UNKNOWN</div>}</Detail>
              <Detail title="Homeworld">{person.homeworld ? <><Fact label="Name" value={person.homeworld.name} /><Fact label="Population" value={person.homeworld.population} /><Fact label="Terrain" value={person.homeworld.terrain} /><Fact label="Climate" value={person.homeworld.climate} /><Fact label="Gravity" value={person.homeworld.gravity} /><Fact label="Diameter" value={person.homeworld.diameter} /></> : <div className="state">UNKNOWN</div>}</Detail>
            </div>
          </div>
          <section className="related"><h2 className="section-title">Films // appearance record</h2><div className="film-row">{films.map(([title, ep], i) => { const f = person.films.find((x) => x.episodeId === i + 1); return <div className={"film " + (f ? "seen" : "")} key={ep}><div className="ep">EPISODE {ep}</div><strong>{title}</strong>{f && <small>CONFIRMED</small>}</div>; })}</div></section>
          <div className="lists related"><Related title={`Starships // ${person.starships.length}`} items={person.starships} /><Related title={`Vehicles // ${person.vehicles.length}`} items={person.vehicles} /></div>
        </>}
      </section>
    </main>
  </>;
}

function Detail({ title, children }) { return <div><h2 className="section-title">{title}</h2><div className="facts">{children}</div></div>; }
function Related({ title, items }) { return <div><h2 className="section-title">{title}</h2>{items.length ? items.map((x) => <div className="item" key={x.id}><div className="thumb" aria-hidden="true">◆</div><div><strong>{x.name}</strong><small>{x.model} · {x.manufacturer}</small><small>Crew / Passengers: {x.crew} / {x.passengers}</small></div></div>) : <div className="state">NO RECORDS</div>}</div>; }
ReactDOM.createRoot(document.getElementById("root")).render(<App />);

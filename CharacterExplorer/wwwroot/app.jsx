const { useEffect, useMemo, useState } = React;

const episodeNames = {
  1: "Episode I",
  2: "Episode II",
  3: "Episode III",
  4: "Episode IV",
  5: "Episode V",
  6: "Episode VI",
};

function Fact({ label, value }) {
  return (
    <div className="fact">
      <label>{label}</label>
      <b>{value || "unknown"}</b>
    </div>
  );
}

function LoadingState({ label }) {
  return (
    <div className="state loading-state" role="status" aria-live="polite">
      <span className="spinner" aria-hidden="true" />
      <span>{label}</span>
    </div>
  );
}

function App() {
  const [people, setPeople] = useState([]);
  const [query, setQuery] = useState("");
  const [selected, setSelected] = useState(null);
  const [person, setPerson] = useState(null);
  const [peopleLoading, setPeopleLoading] = useState(true);
  const [personLoading, setPersonLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    fetch("/api/people")
      .then((response) =>
        response.ok ? response.json() : Promise.reject()
      )
      .then((data) => {
        setPeople(data);
        setSelected(data[0]?.id);
      })
      .catch(() => setError("Unable to load the character index."))
      .finally(() => setPeopleLoading(false));
  }, []);

  useEffect(() => {
    if (!selected) {
      return;
    }

    const controller = new AbortController();
    setPerson(null);
    setPersonLoading(true);

    fetch("/api/people/" + selected, { signal: controller.signal })
      .then((response) =>
        response.ok ? response.json() : Promise.reject()
      )
      .then(setPerson)
      .catch((error) => {
        if (error.name !== "AbortError") {
          setError("Unable to retrieve this character.");
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) {
          setPersonLoading(false);
        }
      });

    return () => controller.abort();
  }, [selected]);

  const filtered = useMemo(
    () =>
      people.filter((personSummary) =>
        personSummary.name.toLowerCase().includes(query.toLowerCase())
      ),
    [people, query]
  );

  return (
    <>
      <header className="top">
        <div className="brand">
          <span aria-hidden="true">&#x2726;</span> HOLOCRON
          <small className="eyebrow">// character index</small>
        </div>
        <div className="search">
          <span aria-hidden="true">&#x2315;</span>
          <input
            aria-label="Search characters"
            placeholder="Search characters"
            value={query}
            onChange={(event) => setQuery(event.target.value)}
          />
        </div>
      </header>

      <main className="layout">
        <aside className="side">
          <h3 className="eyebrow">Characters // {filtered.length}</h3>
          <div className="side-list">
            {peopleLoading ? (
              <LoadingState label="INDEXING..." />
            ) : filtered.length === 0 ? (
              <div className="state">NO MATCHES</div>
            ) : (
              filtered.map((personSummary) => (
                <button
                  className={
                    "person-button " +
                    (selected === personSummary.id ? "active" : "")
                  }
                  onClick={() => setSelected(personSummary.id)}
                  key={personSummary.id}
                >
                  {personSummary.name}
                </button>
              ))
            )}
          </div>
        </aside>

        <section className="main">
          {error && !person && !personLoading ? (
            <div className="state error">{error}</div>
          ) : peopleLoading || personLoading ? (
            <LoadingState label="LOADING DATAPAD..." />
          ) : !person ? (
            <div className="state">SELECT A CHARACTER</div>
          ) : (
            <>
              <div className="card">
                <div className="hero">
                  <div className="avatar" aria-hidden="true">
                    &#x2726;
                  </div>
                  <div>
                    <div className="eyebrow">
                      Datapad // {String(person.id).padStart(3, "0")}
                    </div>
                    <h1>{person.name}</h1>
                  </div>
                </div>

                <div className="grid3">
                  <Detail title="Attributes">
                    <Fact label="Birth year" value={person.birthYear} />
                    <Fact label="Gender" value={person.gender} />
                    <Fact label="Height" value={person.height} />
                    <Fact label="Mass" value={person.mass} />
                    <Fact label="Eye color" value={person.eyeColor} />
                    <Fact label="Hair color" value={person.hairColor} />
                    <Fact label="Skin color" value={person.skinColor} />
                  </Detail>

                  <Detail title="Species">
                    {person.species ? (
                      <>
                        <Fact label="Name" value={person.species.name} />
                        <Fact
                          label="Classification"
                          value={person.species.classification}
                        />
                        <Fact
                          label="Designation"
                          value={person.species.designation}
                        />
                        <Fact
                          label="Average height"
                          value={person.species.averageHeight}
                        />
                        <Fact
                          label="Lifespan"
                          value={person.species.averageLifespan}
                        />
                        <Fact
                          label="Language"
                          value={person.species.language}
                        />
                      </>
                    ) : (
                      <div className="state">UNKNOWN</div>
                    )}
                  </Detail>

                  <Detail title="Homeworld">
                    {person.homeworld ? (
                      <>
                        <Fact label="Name" value={person.homeworld.name} />
                        <Fact
                          label="Population"
                          value={person.homeworld.population}
                        />
                        <Fact
                          label="Terrain"
                          value={person.homeworld.terrain}
                        />
                        <Fact
                          label="Climate"
                          value={person.homeworld.climate}
                        />
                        <Fact
                          label="Gravity"
                          value={person.homeworld.gravity}
                        />
                        <Fact
                          label="Diameter"
                          value={person.homeworld.diameter}
                        />
                      </>
                    ) : (
                      <div className="state">UNKNOWN</div>
                    )}
                  </Detail>
                </div>
              </div>

              <section className="related">
                <h2 className="section-title">Films // appearance record</h2>
                <div className="film-row">
                  {person.films.map((film) => {
                    return (
                      <div
                        className={"film " + (film.appears ? "seen" : "")}
                        key={film.id}
                      >
                        <div className="ep">
                          {episodeNames[film.episodeId] ??
                            `Episode ${film.episodeId}`}
                        </div>
                        <strong>{film.title}</strong>
                        {film.appears && <small>CONFIRMED</small>}
                      </div>
                    );
                  })}
                </div>
              </section>

              <div className="lists related">
                <Related
                  title={`Starships // ${person.starships.length}`}
                  items={person.starships}
                />
                <Related
                  title={`Vehicles // ${person.vehicles.length}`}
                  items={person.vehicles}
                />
              </div>
            </>
          )}
        </section>
      </main>
    </>
  );
}

function Detail({ title, children }) {
  return (
    <div>
      <h2 className="section-title">{title}</h2>
      <div className="facts">{children}</div>
    </div>
  );
}

function Related({ title, items }) {
  return (
    <div>
      <h2 className="section-title">{title}</h2>
      {items.length ? (
        items.map((item) => (
          <div className="item" key={item.id}>
            <div className="thumb" aria-hidden="true">
              &#x25C6;
            </div>
            <div>
              <strong>{item.name}</strong>
              <small>
                {item.model} &#x00B7; {item.manufacturer}
              </small>
              <small>
                Crew / Passengers: {item.crew} / {item.passengers}
              </small>
            </div>
          </div>
        ))
      ) : (
        <div className="state">NO RECORDS</div>
      )}
    </div>
  );
}

ReactDOM.createRoot(document.getElementById("root")).render(<App />);

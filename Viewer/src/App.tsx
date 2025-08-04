import { MapTest } from "./components/MapTest";

function App() {
  return (
    <div className="min-h-screen bg-background">
      <header className="border-b">
        <div className="container mx-auto px-6 py-4">
          <h1 className="text-2xl font-bold">MapGen Viewer</h1>
          <p className="text-muted-foreground">
            Interactive map generation and visualization
          </p>
        </div>
      </header>

      <main className="py-8">
        <MapTest />
      </main>
    </div>
  );
}

export default App;

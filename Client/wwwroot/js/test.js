$.ajax({
    url: "https://pokeapi.co/api/v2/pokemon/"
}).done((result) => {
    const pokemonGrid = $("#pokemonGrid");

    $.each(result.results, (key, val) => {
        // Membuat tampilan grid untuk setiap Pokémon
        const pokemonCard = `
            <div class="col-md-4">
                <div class="card mb-4 shadow">
                    <img src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/${key + 1}.png" class="card-img-top" alt="${val.name}">
                    <div class="card-body">
                        <h5 class="card-title text-capitalize">${val.name}</h5>
                        <button type="button" onclick="detail('${val.url}')" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalPoke">Detail</button>
                    </div>
                </div>
            </div>
        `;
        // Menambahkan kartu Pokémon ke dalam grid
        pokemonGrid.append(pokemonCard);
    });
}).fail((error) => {
    console.log(error);
});


function detail(stringUrl) {
    $.ajax({
        url: stringUrl
    }).done((res) => {
        $(".modal-title").text(res.name);
        $(".pokeImage img").attr("src", res.sprites.other.dream_world.front_default);

        const pokeType = res.types.map((type) => {
            return typeColor(type.type.name);
        }).join("");
        $(".pokeType").html(pokeType);

        const pokeAbility = res.abilities.map((ability) => {
            return `<h6 class="text-capitalize mt-3">${ability.ability.name}</h6>`;
        }).join("");
        $(".pokeAbility").html(pokeAbility);

        // Mengambil data moves (pergerakan) dari API
        const moveList = res.moves.slice(0, 5); // Ambil 5 move pertama
        const moveListItems = moveList.map((moveData) => {
            // Ambil data move dari URL yang ada di moveData
            $.ajax({
                url: moveData.move.url
            }).done((move) => {
                const moveItem = `<li><strong>${move.name}</strong>`;
                $("#modal-pokemon-moves").append(moveItem);
            }).fail((error) => {
                console.log(error);
            });
        });

        // Bersihkan daftar pergerakan sebelum menambahkannya
        $("#modal-pokemon-moves").html("");

        const statNameValue = res.stats.map((stat) => {
            let barColor = '';

            switch (stat.stat.name) {
                case 'hp':
                    barColor = 'bg-success'; // Warna untuk HP
                    break;
                case 'attack':
                    barColor = 'bg-danger'; // Warna untuk Attack
                    break;
                case 'defense':
                    barColor = 'bg-primary'; // Warna untuk Defense
                    break;
                case 'special-attack':
                    barColor = 'bg-warning'; // Warna untuk Special Attack
                    break;
                case 'special-defense':
                    barColor = 'bg-info'; // Warna untuk Special Defense
                    break;
                case 'speed':
                    barColor = 'bg-dark'; // Warna untuk Speed
                    break;
                default:
                    barColor = 'bg-secondary'; // Warna default jika tidak ada yang cocok
            }

            return `
        <div class="row mt-3">
            <div class="col d-inline font-weight-bold text-left text-capitalize">${stat.stat.name}</div>
            <div class="col d-inline font-weight-bold text-right">${stat.base_stat}</div>
        </div>
        <div class="row progress mb-3 mx-auto">
            <div class="progress-bar ${barColor}" role="progressbar" style="width: ${stat.base_stat}%" aria-valuenow="${stat.base_stat}" aria-valuemin="0" aria-valuemax="100">
            </div>
        </div>
    `;
        }).join("");
        $(".statNameValue").html(statNameValue);


    }).fail((error) => {
        console.log(error);
    });
}

function getMoveDescription(moveData) {
    const effectEntry = moveData.effect_entries.find(entry => entry.language.name === "en");
    return effectEntry ? effectEntry.effect : "Tidak ada deskripsi tersedia.";
}

function typeColor(type) {
    const typeColors = {
        "normal": "bg-light text-dark",
        "fighting": "bg-danger text-white",
        // ... and other types as you provided earlier
    };

    return `<h6 class="${typeColors[type]} rounded-sm d-inline p-1 mr-1">${type}</h6>`;
}
